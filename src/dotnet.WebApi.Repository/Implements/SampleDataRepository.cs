using System.Data;
using Dapper;
using dotnet.WebApi.Repository.Db.SampleDb.Entities;
using dotnet.WebApi.Repository.Enums;
using dotnet.WebApi.Repository.Interfaces;

namespace dotnet.WebApi.Repository.Implements;

/// <summary>
/// Sample Data Repository
/// </summary>
public class SampleDataRepository : ISampleDataRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly IUnitOfWork _unitOfWorkFunc;

    public SampleDataRepository(Func<DbEnum, IDbConnection> dbConnectFunc, Func<DbEnum, IUnitOfWork> unitOfWorkFunc)
    {
        this._unitOfWorkFunc = unitOfWorkFunc.Invoke(DbEnum.SampleDb);
        this._dbConnection = dbConnectFunc.Invoke(DbEnum.SampleDb);
    }

    /// <summary>
    /// 用 Dapper 取得資料
    /// </summary>
    /// <param name="serialId"></param>
    /// <returns></returns>
    public async Task<SampleData?> GetAsync(int serialId)
    {
        var tempSql = @"select * from SampleData where serialId = @serialId";

        var parameter = new DynamicParameters();
        parameter.Add("serialId", serialId);

        return await this._dbConnection.QueryFirstOrDefaultAsync<SampleData>(tempSql, parameter);
    }

    /// <summary>
    /// 用 EFCore 存檔
    /// </summary>
    /// <param name="sampleData"></param>
    /// <returns></returns>
    public async Task<int> SaveAsync(SampleData sampleData)
    {
        var repository = this._unitOfWorkFunc.GetRepository<SampleData>();
        repository.Add(sampleData);

        await this._unitOfWorkFunc.SaveChangesAsync();
        return sampleData.SerialId;
    }
}