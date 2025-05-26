# dotnetLab.Analyzers

這個專案包含一組用於 Domain-Driven Design (DDD) 設計規範檢查的 Roslyn 分析器。

## 分析器列表

### 1. ValueObjectMustBeImmutableAnalyzer

**規則 ID:** DDD001
**嚴重性:** Warning

檢查所有實作 `IValueObject` 的類型是否遵循不可變原則。值物件的所有屬性都應該是唯讀的。

#### 違規範例:
