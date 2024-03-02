using dotnetLab.DomainEntities;

namespace dotnetLab.Application.SimpleDocument.Ports.Out;

/// <summary>
/// 範例資料 Repository
/// </summary>
public interface ISimpleDocumentRepository
{
    /// <summary>
    /// 依據 serialId 取得資料
    /// </summary>
    /// <param name="serialId"></param>
    /// <returns></returns>
    Task<SimpleDocumentEntity?> GetAsync(int serialId);

    /// <summary>
    /// 儲存文件
    /// </summary>
    /// <param name="sampleTable"></param>
    /// <returns></returns>
    Task<int> SaveAsync(SimpleDocumentEntity sampleTable);
}