<#
.SYNOPSIS
以已還原的 Domain 專案驗證值物件診斷的 warning/error/none 與修復流程。
.DESCRIPTION
先在相同 NUGET_PACKAGES、UseArtifactsOutput、ArtifactsPath 環境完成 solution restore。
所有反例、診斷及編譯輸出必須位於使用者指定的 F: RAM disk 任務目錄。
本腳本不修改正式來源、套件版本或全域設定；正常建置不會載入反例。
#>
[CmdletBinding()]
param([Parameter(Mandatory)][string]$ScratchRoot)

$ErrorActionPreference = 'Stop'
$scratch = [IO.Path]::GetFullPath($ScratchRoot).TrimEnd('\', '/')
$drive = [IO.DriveInfo]::new('F:\')
if (-not $scratch.StartsWith('F:\', [StringComparison]::OrdinalIgnoreCase) -or $scratch.Length -le 3 -or
    -not $drive.IsReady -or $drive.VolumeLabel -ne 'RAMDRIVE') {
    throw 'ScratchRoot 必須是已確認 F: RAMDRIVE 下的獨立任務目錄。'
}
if ($env:UseArtifactsOutput -ne 'true' -or -not $env:ArtifactsPath -or -not $env:NUGET_PACKAGES) {
    throw '請先設定 F: 上的 UseArtifactsOutput=true、ArtifactsPath 與 NUGET_PACKAGES 並完成還原。'
}
foreach ($path in @($env:ArtifactsPath, $env:NUGET_PACKAGES, $env:TEMP, $env:TMP)) {
    if (-not $path -or -not [IO.Path]::GetFullPath($path).StartsWith('F:\', [StringComparison]::OrdinalIgnoreCase)) {
        throw '編譯輸出、套件快取與程序 TEMP/TMP 必須全部位於 F:。'
    }
}

$probeDirectory = Join-Path $scratch 'value-object-probe'
New-Item -ItemType Directory -Path $probeDirectory -Force | Out-Null
$project = Join-Path $PSScriptRoot '../src/dotnetLab.Domain/dotnetLab.Domain.csproj'
$source = Join-Path $probeDirectory 'Probe.cs'
$targets = Join-Path $probeDirectory 'Inject.targets'
$config = Join-Path $probeDirectory '.editorconfig'
$globalConfig = Join-Path $probeDirectory 'coverage.globalconfig'
@'
using ArchPrecept.Contracts.Domain;
using dotnetLab.Domain.Products;

namespace AdoptionProbe;

internal sealed class InvalidValueObject : ValueObject
{
    public int Quantity { get; set; }
    public Product? Product { get; init; }
    public override IEnumerable<object> GetEqualityComponents() => Array.Empty<object>();
}

internal sealed class UnknownValueObject : ValueObject
{
    public object? Payload { get; init; }
    public override IEnumerable<object> GetEqualityComponents() => Array.Empty<object>();
}
'@ | Set-Content -LiteralPath $source
@'
is_global = true
global_level = 200
dotnet_diagnostic.DBA0002.severity = warning
'@ | Set-Content -LiteralPath $globalConfig
@'
<Project>
  <Target Name="InjectAdoptionProbe" BeforeTargets="CoreCompile">
    <ItemGroup>
      <Compile Include="$(MSBuildThisFileDirectory)Probe.cs" Condition="'$(IncludeAdoptionProbe)' == 'true'" />
      <EditorConfigFiles Include="$(MSBuildThisFileDirectory).editorconfig" />
      <EditorConfigFiles Include="$(MSBuildThisFileDirectory)coverage.globalconfig" />
    </ItemGroup>
  </Target>
</Project>
'@ | Set-Content -LiteralPath $targets

function Invoke-ProbeBuild([string]$Name, [string]$Severity, [bool]$IncludeProbe) {
    @"
root = true
[*.cs]
dotnet_diagnostic.DBA1018.severity = $Severity
dotnet_diagnostic.DBA1019.severity = $Severity
"@ | Set-Content -LiteralPath $config
    $sarif = Join-Path $probeDirectory "$Name.sarif"
    $log = Join-Path $probeDirectory "$Name.log"
    if (Test-Path -LiteralPath $sarif) { Remove-Item -LiteralPath $sarif }
    $include = $IncludeProbe.ToString().ToLowerInvariant()
    & dotnet build $project -c Release -t:Rebuild --no-restore -nr:false -m:1 `
        -p:UseAppHost=false -p:UseSharedCompilation=false `
        "-p:CustomAfterMicrosoftCommonTargets=$targets" "-p:IncludeAdoptionProbe=$include" "-p:ErrorLog=$sarif" *> $log
    $buildExit = $LASTEXITCODE
    if (-not (Test-Path -LiteralPath $sarif)) { throw "未產生 $sarif；請查看 $log" }
    $report = Get-Content -LiteralPath $sarif -Raw | ConvertFrom-Json
    $diagnostics = @($report.runs[0].results)
    $vo = @($diagnostics | Where-Object { $_.ruleId -in @('DBA1018', 'DBA1019') })
    $unexpectedErrors = @($diagnostics | Where-Object { $_.level -eq 'error' -and $_.ruleId -notin @('DBA1018', 'DBA1019') })
    $expectedExit = if ($IncludeProbe -and $Severity -eq 'error') { 1 } else { 0 }
    if ($buildExit -ne $expectedExit -or $unexpectedErrors.Count) { throw "$Name 的退出碼或額外錯誤不符；請查看 $log" }
    if ($IncludeProbe -and $Severity -ne 'none') {
        foreach ($id in @('DBA1018', 'DBA1019')) {
            if (-not ($vo | Where-Object { $_.ruleId -eq $id -and $_.level -eq $Severity })) {
                throw "$Name 缺少 $id/$Severity 診斷。"
            }
        }
    } elseif ($vo.Count) { throw "$Name 不應含值物件違規診斷。" }
    if ($diagnostics | Where-Object { $_.ruleId -in @('DBA0001', 'AD0001', 'CS8032', 'CS9057') }) {
        throw "$Name 存在設定或 Analyzer 載入／執行失敗。"
    }
    $coverage = @($diagnostics | Where-Object ruleId -eq 'DBA0002' |
        ForEach-Object { $_.properties.customProperties })
    foreach ($type in @('Address', 'OrderLineSnapshot', 'OrderSnapshot')) {
        foreach ($id in @('DBA1018', 'DBA1019')) {
            if (-not ($coverage | Where-Object {
                $_.type -eq "dotnetLab.Domain.Shipments.ValueObjects.$type" -and $_.diagnosticId -eq $id -and
                $_.status -eq 'applicable' -and $_.roleEvidence -eq 'inherited-contract' -and $_.confidence -eq 'high'
            })) { throw "$Name 未取得 $type/$id 的精確繼承契約適用性證據。" }
        }
    }
    if (-not ($coverage | Where-Object {
        $_.type -eq 'dotnetLab.Domain.Shipments.ValueObjects.OrderSnapshot' -and
        $_.diagnosticId -eq 'DBA1018' -and $_.status -eq 'not-evaluated'
    })) { throw "$Name 未明列 OrderSnapshot 唯讀集合的不可變性覆蓋限制。" }
    if ($IncludeProbe) {
        foreach ($id in @('DBA1018', 'DBA1019')) {
            if (-not ($coverage | Where-Object {
                $_.type -eq 'AdoptionProbe.UnknownValueObject' -and $_.diagnosticId -eq $id -and $_.status -eq 'not-evaluated'
            })) { throw "$Name 的未知 object 狀態不應被判為通過。" }
        }
    }
    [pscustomobject]@{
        Case = $Name
        ExitCode = $buildExit
        DBA1018 = @($vo | Where-Object ruleId -eq 'DBA1018').Count
        DBA1019 = @($vo | Where-Object ruleId -eq 'DBA1019').Count
        CoverageDiagnostics = @($diagnostics | Where-Object ruleId -eq 'DBA0002').Count
        NotEvaluated = @($coverage | Where-Object status -eq 'not-evaluated').Count
        Sarif = $sarif
    }
}

$results = @(
    Invoke-ProbeBuild 'positive' 'warning' $false
    Invoke-ProbeBuild 'warning' 'warning' $true
    Invoke-ProbeBuild 'error' 'error' $true
    Invoke-ProbeBuild 'none' 'none' $true
    Invoke-ProbeBuild 'recovered' 'warning' $false
)
$results | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $probeDirectory 'results.json')
$results | Format-Table Case, ExitCode, DBA1018, DBA1019, CoverageDiagnostics, NotEvaluated
