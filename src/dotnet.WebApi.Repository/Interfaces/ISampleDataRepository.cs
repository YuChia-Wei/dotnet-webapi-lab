using dotnet.WebApi.Repository.Db.SampleDb.Entities;

namespace dotnet.WebApi.Repository.Interfaces;

/// <summary>
/// 範例資料 Repository
/// </summary>
public interface ISampleDataRepository
{
    /// <summary>
    /// 依據 serialId 取得資料
    /// </summary>
    /// <param name="serialId"></param>
    /// <returns></returns>
    Task<SampleData> GetAsync(int serialId);

    Task<int> SaveAsync(SampleData sampleData);
}