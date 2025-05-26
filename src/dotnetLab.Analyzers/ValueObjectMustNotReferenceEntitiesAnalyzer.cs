using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace dotnetLab.Analyzers;

/// <summary>
/// 值物件不應引用實體的分析器
/// 檢查所有實作 IValueObject 的類型是否包含對 IDomainEntity 的引用
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ValueObjectMustNotReferenceEntitiesAnalyzer : DiagnosticAnalyzer
{
    // 診斷ID和訊息
    public const string DiagnosticId = "DDD002";
    private const string Category = "DomainDrivenDesign";
    private static readonly LocalizableString Title = "值物件不應引用實體";
    private static readonly LocalizableString MessageFormat = "值物件 '{0}' 包含對實體類型 '{1}' 的引用，值物件應只引用其他值物件或基本型別";
    private static readonly LocalizableString Description = "值物件不應引用實體，應只引用其他值物件或基本型別。";

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

        // 檢查所有屬性的型別
        foreach (var property in namedTypeSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            if (property.IsStatic)
            {
                continue;
            }

            var propertyType = property.Type;

            // 檢查屬性型別是否為實體或集合型別
            if (this.IsEntityType(propertyType) || this.IsCollectionOfEntityType(propertyType, context.Compilation))
            {
                // 報告引用實體的診斷
                var diagnostic = Diagnostic.Create(
                    Rule,
                    property.Locations[0],
                    namedTypeSymbol.Name,
                    propertyType.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private bool IsCollectionOfEntityType(ITypeSymbol typeSymbol, Compilation compilation)
    {
        // 檢查是否為集合型別
        if (typeSymbol is INamedTypeSymbol namedType && this.IsCollectionType(namedType, compilation))
        {
            // 如果是泛型集合，檢查泛型參數是否為實體
            if (namedType.IsGenericType && namedType.TypeArguments.Length > 0)
            {
                return this.IsEntityType(namedType.TypeArguments[0]);
            }
        }

        return false;
    }

    private bool IsCollectionType(INamedTypeSymbol typeSymbol, Compilation compilation)
    {
        // 檢查是否實作 IEnumerable<T>
        var enumerableType = compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1");
        if (enumerableType != null)
        {
            foreach (var interfaceSymbol in typeSymbol.AllInterfaces)
            {
                if (interfaceSymbol.OriginalDefinition.Equals(enumerableType, SymbolEqualityComparer.Default))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsEntityType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol == null)
        {
            return false;
        }

        // 檢查是否實作 IDomainEntity 介面
        if (typeSymbol is INamedTypeSymbol namedType)
        {
            foreach (var interfaceSymbol in namedType.AllInterfaces)
            {
                if (interfaceSymbol.Name.StartsWith("IDomainEntity") &&
                    interfaceSymbol.ContainingNamespace.ToString().EndsWith("Entities"))
                {
                    return true;
                }
            }
        }

        return false;
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