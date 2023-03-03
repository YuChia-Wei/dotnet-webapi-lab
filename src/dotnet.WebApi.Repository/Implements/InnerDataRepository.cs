using dotnet.WebApi.AopComponent.Attributes;
using dotnet.WebApi.Repository.Db.SampleDb.Entities;
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
    public Task<SampleData> GetAsync(int serialId)
    {
        return Task.FromResult(new SampleData
        {
            SerialId = serialId, Description = "for no db test", DocumentNum = $"no{serialId}"
        });
    }

    public Task<int> SaveAsync(SampleData sampleData)
    {
        return Task.FromResult(123);
    }
}