using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace dotnetLab.Analyzers;

/// <summary>
/// 值物件不可變性分析器
/// 檢查所有實作 IValueObject 的類型是否遵循不可變原則
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ValueObjectMustBeImmutableAnalyzer : DiagnosticAnalyzer
{
    // 診斷ID和訊息
    public const string DiagnosticId = "DDD001";
    private const string Category = "DomainDrivenDesign";
    private static readonly LocalizableString Title = "值物件必須是不可變的";
    private static readonly LocalizableString MessageFormat = "值物件 '{0}' 包含可變屬性 '{1}'，所有值物件屬性必須是唯讀的";
    private static readonly LocalizableString Description = "值物件必須是不可變的，所有屬性都應該是唯讀的。";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        true,
        Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // 註冊符號分析
        context.RegisterSymbolAction(this.AnalyzeSymbol, SymbolKind.NamedType);
    }

    private void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        // 檢查是否實作 IValueObject 介面
        if (!this.IsValueObject(namedTypeSymbol))
        {
            return;
        }

        // 檢查所有屬性是否是唯讀的
        foreach (var property in namedTypeSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            // 忽略靜態屬性、只讀屬性和自動實作的屬性
            if (property.IsStatic ||
                property.IsReadOnly ||
                property.SetMethod == null ||
                !property.SetMethod.DeclaredAccessibility.HasFlag(Accessibility.Public))
            {
                continue;
            }

            // 報告可變屬性的診斷
            var diagnostic = Diagnostic.Create(
                Rule,
                property.Locations[0],
                namedTypeSymbol.Name,
                property.Name);

            context.ReportDiagnostic(diagnostic);
        }
    }

    private bool IsValueObject(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol == null)
        {
            return false;
        }

        // 檢查是否直接實作 IValueObject
        foreach (var interfaceSymbol in typeSymbol.AllInterfaces)
        {
            if (interfaceSymbol.Name == "IValueObject" &&
                interfaceSymbol.ContainingNamespace.ToString().EndsWith("ValueObjects"))
            {
                return true;
            }
        }

        return false;
    }
}