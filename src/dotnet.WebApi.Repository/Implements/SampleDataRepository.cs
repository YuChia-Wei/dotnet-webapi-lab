using System.Data;
using Dapper;
using dotnet.WebApi.Database.SampleDb.Entities;
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
    public async Task<SampleTable> GetAsync(int serialId)
    {
        var tempSql = @"select * from SampleTable where serialId = @serialId";

        var parameter = new DynamicParameters();
        parameter.Add("serialId", serialId);

        return await this._dbConnection.QueryFirstOrDefaultAsync<SampleTable>(tempSql, parameter);
    }

    /// <summary>
    /// 用 EFCore 存檔
    /// </summary>
    /// <param name="sampleTable"></param>
    /// <returns></returns>
    public async Task<int> SaveAsync(SampleTable sampleTable)
    {
        var repository = this._unitOfWorkFunc.GetRepository<SampleTable>();
        repository.Add(sampleTable);

        await this._unitOfWorkFunc.SaveChangesAsync();
        return sampleTable.SerialId;
    }
}