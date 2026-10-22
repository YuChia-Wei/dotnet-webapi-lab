# dotnet Lab

## Authentication Server (keycloak) prepare

- docker run
    ```bash
    docker run -d -p 8080:8080 -p 8443:8443 \
        -e KEYCLOAK_ADMIN=user \
        -e KEYCLOAK_ADMIN_PASSWORD=password \
        quay.io/keycloak/keycloak start-dev
    ```
- import test client
    - client id testing json : [link](./keycloak/test_client.json)
    - open clients page</br>
        ![keycloak-clients-page.png](./img/keycloak-clients-page.png)
    - click import client</br>
        ![keycloak-clients-import-button.png](./img/keycloak-clients-import-button.png)
- set client scopes mappers
  - audience mapper</br>
    ![keycloak-client-scopes-mapper-audience.png](./img/keycloak-client-scopes-mapper-audience.png)
  - user name</br>
    ![keycloak-client-scopes-mapper-username.png](./img/keycloak-client-scopes-mapper-username.png)

### keycloak export

**這邊的 keycloak 都是使用 dev mode 啟動，所以使用的是 H2 資料庫**

**匯出命令最後沒有加上 `--users <mode>` 參數的目的是要使用預設的 `different_files` 方式匯出，其他還有以下模式**
- --users realm_file：把使用者寫進同一個 realm 檔（單一 JSON），所以看起來「合併在一起」。
- --users same_file：realm 一個檔、所有使用者集中在另一個單獨檔（兩個檔）。
- --users different_files（預設）：realm 一個檔、使用者分割成多個檔（*-users-0.json, *-users-1.json…），可用 --users-per-file 控制每檔人數。
- --users skip：不匯出使用者。

**這邊偷懶所有設定都直接在 master，因此是匯出 master，如果真的要實際在自己的服務中使用，建議重新審視設定**

#### 方法 1

- 建立當下容器的快照
```shell
docker commit keycloak kc-snap:export
#             <要快照的容器> <快照後的映象名稱>
```
- 使用快照執行備份指令
```shell
docker run --rm --name kc-export -v ${PWD}\keycloak\backup:/tmp/backup kc-snap:export export --dir /tmp/backup --realm master
```

#### 方法 2

- 要備份的 keycloak 容器需要將 `/opt/keycloak/data` 路徑掛到 volumes 上
- 使用另外一個 keycloak 容器連到同一個 volumes 並執行備份命令
```shell
docker run --rm --name kc-export --volumes-from keycloak -v ./keycloak/backup:/tmp/backup quay.io/keycloak/keycloak:26.3 export --dir /tmp/backup --realm master
```

#### ref

- https://stackoverflow.com/questions/72128765/import-realm-in-keycloak18-x
- https://www.keycloak.org/server/containers

## architecture ref

- https://romanglushach.medium.com/hexagonal-architecture-the-secret-to-scalable-and-maintainable-code-for-modern-software-d345fdb47347
- https://en.wikipedia.org/wiki/Hexagonal_architecture_(software)
- https://medium.com/idealo-tech-blog/hexagonal-ports-adapters-architecture-e3617bcf00a0

## 專案結構與 ArchPrecept

本方案使用已發布的 **ArchPrecept 0.0.3-preview**，透過 GitHub Packages 還原；`nuget.config` 只保存來源與 package mapping，不保存憑證。已有 `github-yuchia` 認證的開發機可沿用設定；新環境須透過 NuGet credential provider、環境變數或私人設定檔提供 `read:packages` 認證。

| 專案 | 責任與主要相依 |
| --- | --- |
| `dotnetLab.Domain` | 業務聚合、實體、值物件及領域事件；只引用 `ArchPrecept.Contracts.Domain`。 |
| `dotnetLab.Application` | 業務 Use Case、輸入與 DTO、Wolverine 訊息轉接器；使用 Domain 及 `ArchPrecept.Contracts.Application`。 |
| `dotnetLab.Infrastructure` | 聚合儲存、唯讀查詢、gRPC 與 Wolverine 領域事件分派 adapter。 |
| `dotnetLab.Persistence.Metadata.SampleDb` | EF Core 的資料表模型與資料庫 mapping。 |
| `dotnetLab.WebApi` | HTTP Request／ViewModel、驗證、認證及標準回應包裝；Controller 呼叫 Use Case。 |
| `dotnetLab.GrpcService` | gRPC 入口；透過相同的應用程式 Use Case 查詢。 |
| `dotnetLab.CrossCutting.Observability` | Tracing 與 metrics。 |
| `dotnetLab.Tests` | 領域 invariant、Use Case 保存／事件順序、真實主機的程序內 HTTP 回歸。 |

- Aggregate／Entity 直接繼承套件 `AggregateRoot<TId>`／`DomainEntity<TId>`，所有領域事件實作套件 `IDomainEvent`。
- 值物件繼承套件 `ValueObject`，明列相等性元件；訂單快照按項目內容比較。
- 儲存 adapter 使用套件 `IAggregateRepository<TAggregate,TId>`；文件唯讀 Port 繼承 `IQueryRepository`，回傳 DTO。
- 業務專用 Use Case／查詢 Port、產品物流資訊、資料庫連線工廠、API 權限介面仍由本專案擁有。套件沒有提供可取代這些業務／技術責任的介面。
- 已移除重複的 BuildingBlocks 與未接線的本地 Analyzer 專案；未使用的 `IRepository`／`ISpecification` 一併移除。舊 DDD001／DDD002 的值物件設計意圖由新 DBA1018／DBA1019 提供有界檢查；新規則不是舊實作的逐行移植，也不是完整深層不可變性證明。

### 建置、測試與診斷

需要 `global.json` 指定的 .NET 10 SDK。一般 CLI 即可執行，不需 AI、API key 或 framework skill：

```powershell
dotnet restore dotnet-lab.slnx
dotnet build dotnet-lab.slnx -c Release --no-restore
dotnet test dotnet-lab.slnx -c Release --no-build --no-restore
dotnet format dotnet-lab.slnx --no-restore --verify-no-changes
```

`src/Directory.Build.targets` 將 `ArchPrecept.Analyzers.Core` 以 `PrivateAssets=all` 加到所有正式專案；測試專案除外。Analyzer 載入／執行失敗 CS8032、CS9057、AD0001 會使建置失敗。`.editorconfig` 保留 DBA1001–DBA1017 為 error，新增 DBA1018／DBA1019 為 warning。

`src/archprecept.repository.xml` 明確選用 `core,web-api,messaging,event-sourcing,framework,ddd`，保留原 17 條規則及既有 Use Case／薄 Handler 慣例，再加入兩條值物件規則。Profile 選用不會安裝 ES、Messaging 或 AI Framework 套件。一般角色沿用明確啟用的命名慣例；VO／Entity 必須具有精確套件契約或 mapping 證據。Domain 的 project 設定將三個物流值物件列為 Expected。

0.0.3-preview 已修正 package-owned `IDomainEventDispatcher` 的 DBA1016 誤報，`UpdateSimpleDocumentDescriptionUseCase` 已移除局部 pragma。保存／分派／清除的順序及失敗行為由回歸測試保護。

升級時 DBA1001 新增檢查具體 repository 類別；既有兩個文件 adapter 同時實作聚合與查詢 Port，因此公開的 `FindByIdAsync`／`SaveAsync` 被當成 query-repository 成員。依 [DBA1001 明列的類別層級政策](https://github.com/YuChia-Wei/ArchPrecept/blob/main/docs/diagnostics/DBA1001.md)，本專案將 DTO 查詢轉接抽為 `SimpleDocumentQueryRepository`，仍使用同一 scoped 聚合儲存庫。這是採用套件政策的相容調整，不表示共用具體 adapter 實作兩個隔離 Port 普遍違反 DDD／Clean Architecture；新版文件仍可補充雙 Port adapter 的明確升級範例。

值物件驗收可用 [eng/Test-ArchPreceptAdoption.ps1](eng/Test-ArchPreceptAdoption.ps1) 重現。先將程序 TEMP／TMP、NuGet caches、`UseArtifactsOutput=true` 與 `ArtifactsPath` 設在已確認的 F: RAM disk 任務目錄並完成 restore，再執行 `./eng/Test-ArchPreceptAdoption.ps1 -ScratchRoot F:\tmp\<task-directory>`。腳本只暫時向 Domain 編譯加入 F: 反例，檢查 warning／error／none 與移除反例後的結果，並以 DBA0002 warning 輸出 SARIF。

`Address`、`OrderLineSnapshot`、`OrderSnapshot` 的角色均由繼承套件契約辨識。`OrderSnapshot.OrderLines` 的 `IReadOnlyCollection<T>` 不能讓靜態分析器證明 backing state 不可變，因此 DBA1018 會另回報 not-evaluated；保留目前防禦性複製與唯讀包裝，使用行為測試驗證來源集合改動不影響快照。局部 not-evaluated 優先於一般 applicable，未知型別也不算通過；零診斷不代表完整架構或深層不可變性合格。

已知安全相依修正為 `Microsoft.OpenApi` 2.7.5 與 `System.Security.Cryptography.Pkcs` 9.0.21，分別對應 [OpenAPI 公告](https://github.com/advisories/GHSA-v5pm-xwqc-g5wc) 與 [Pkcs 公告](https://github.com/advisories/GHSA-846f-f764-76mx)。這是相依版本修正，不代表已部署服務完成更新。

### 範例 adapter 的範圍

預設 `AddDataSource()` 使用不需外部服務的 mock adapter，既有正整數／非空 Guid 查詢可產生示範資料。保存只在目前 DI scope 中保留狀態；不保證跨 HTTP 要求或程序重啟後仍可讀取。文件新建示範序號從 123 開始。

SQL adapter 使用 Dapper 查詢及 EF Core 保存，新增時把實際資料庫識別碼回寫到聚合，並實作更新；切換前仍須設定連線與 DI。gRPC 範例協定只有查詢，因此 adapter 僅實作唯讀 Port，不再以 `NotImplementedException` 假裝支援新增或更新。

事件在保存成功後才交給 `IDomainEventDispatcher`，分派成功才清除待處理集合。Wolverine adapter 提供程序內分派，不提供 Outbox、資料庫與訊息原子提交、去重或 exactly-once 保證。真實資料庫、遠端 gRPC、Keycloak 及 broker 整合須另行驗收。

### 容器建置

兩個 Dockerfile 均以 repository 根目錄為 build context，以取得 `global.json`、`nuget.config` 與共用 Analyzer 引用：

```shell
docker build -f src/dotnetLab.WebApi/Dockerfile --secret id=nuget_config,src=/private/NuGet.Config .
docker build -f src/dotnetLab.GrpcService/Dockerfile --secret id=nuget_config,src=/private/NuGet.Config .
```

`nuget_config` 必須含 Linux runner 可用的 package sources、mapping 與 GitHub Packages 讀取認證，透過 BuildKit secret 掛載，不寫進映像層。不能直接把 Windows 加密的 NuGet 憑證當成 Linux 已驗收認證；本次不啟動容器或外部服務。
