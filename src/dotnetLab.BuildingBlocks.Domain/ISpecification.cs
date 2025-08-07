namespace dotnetLab.BuildingBlocks.Domain;

/// <summary>
/// 表示規格模式的介面
/// 規格模式用於封裝業務規則，使得這些規則可以被組合和重用
/// </summary>
/// <typeparam name="T">規格適用的實體類型</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// 判斷指定的實體是否滿足規格
    /// </summary>
    /// <param name="entity">要檢查的實體</param>
    /// <returns>如果實體滿足規格，則為 true；否則為 false</returns>
    bool IsSatisfiedBy(T entity);

    /// <summary>
    /// 將此規格與另一個規格進行邏輯「與」操作
    /// </summary>
    /// <param name="other">要組合的另一個規格</param>
    /// <returns>組合後的新規格</returns>
    ISpecification<T> And(ISpecification<T> other);

    /// <summary>
    /// 將此規格與另一個規格進行邏輯「或」操作
    /// </summary>
    /// <param name="other">要組合的另一個規格</param>
    /// <returns>組合後的新規格</returns>
    ISpecification<T> Or(ISpecification<T> other);

    /// <summary>
    /// 對此規格進行邏輯「非」操作
    /// </summary>
    /// <returns>邏輯否定後的新規格</returns>
    ISpecification<T> Not();
}
