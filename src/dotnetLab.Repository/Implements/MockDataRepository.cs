using dotnetLab.Application.SimpleDocument.Ports.Out;
using dotnetLab.Domain;
using dotnetLab.Observability.Tracing;

namespace dotnetLab.Repository.Implements;

[TracingMethod]
public class MockDataRepository : ISimpleDocumentRepository
{
    /// <summary>
    /// 依據 serialId 取得資料
    /// </summary>
    /// <param name="serialId"></param>
    /// <returns></returns>
    public Task<SimpleDocumentEntity?> GetAsync(int serialId)
    {
        return Task.FromResult(new SimpleDocumentEntity
        {
            SerialId = serialId,
            Description = "for no db test",
            DocumentNum = $"no{serialId}"
        });
    }

    public Task<int> SaveAsync(SimpleDocumentEntity simpleDocument)
    {
        return Task.FromResult(123);
    }
}