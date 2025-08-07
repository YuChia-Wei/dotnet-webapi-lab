namespace dotnetLab.Application.Products.Queries;

/// <summary>
/// 取得商品查詢
/// </summary>
public class GetProductQuery
{
    /// <summary>
    /// 商品識別碼
    /// </summary>
    public Guid ProductId { get; set; }
}