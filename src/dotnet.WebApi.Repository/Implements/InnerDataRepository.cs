using dotnet.WebApi.Observability.Tracing;
using dotnet.WebApi.Database.SampleDb.Entities;
using dotnet.WebApi.Repository.Interfaces;

namespace dotnet.WebApi.Repository.Implements;

[OpenTelemetryTracing]
public class InnerDataRepository : ISampleDataRepository
{
    /// <summary>
    /// 依據 serialId 取得資料
    /// </summary>
    /// <param name="serialId"></param>
    /// <returns></returns>
    public Task<SampleTable> GetAsync(int serialId)
    {
        return Task.FromResult(new SampleTable
        {
            SerialId = serialId, Description = "for no db test", DocumentNum = $"no{serialId}"
        });
    }

    public Task<int> SaveAsync(SampleTable sampleTable)
    {
        return Task.FromResult(123);
    }
}