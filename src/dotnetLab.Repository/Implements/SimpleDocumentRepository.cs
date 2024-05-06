using System.Data;
using Dapper;
using dotnetLab.Database.SampleDb;
using dotnetLab.Database.SampleDb.Entities;
using dotnetLab.DomainEntity;
using dotnetLab.Repository.Enums;
using dotnetLab.UseCase.SimpleDocument.Ports.Out;

namespace dotnetLab.Repository.Implements;

/// <summary>
/// Simple Document Repository
/// </summary>
public class SimpleDocumentRepository : ISimpleDocumentRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly SampleDbContext _dbContext;

    public SimpleDocumentRepository(Func<DbInstanceEnum, IDbConnection> dbConnectFunc, SampleDbContext dbContext)
    {
        this._dbContext = dbContext;
        this._dbConnection = dbConnectFunc.Invoke(DbInstanceEnum.SamplePostgreSQL);
    }

    /// <summary>
    /// 用 Dapper 取得資料
    /// </summary>
    /// <param name="serialId"></param>
    /// <returns></returns>
    public async Task<SimpleDocumentEntity?> GetAsync(int serialId)
    {
        var tempSql = "select * from SimpleDocument where serialId = @serialId";

        var parameter = new DynamicParameters();
        parameter.Add("serialId", serialId);

        var result = await this._dbConnection.QueryFirstOrDefaultAsync<SimpleDocumentEntity>(tempSql, parameter);

        return result;
        // return new SimpleDocumentEntity { SerialId = result.SerialId, Description = result.Description, DocumentNum = result.DocumentNum };
    }

    /// <summary>
    /// 用 EFCore 存檔
    /// </summary>
    /// <param name="simpleDocument"></param>
    /// <returns></returns>
    public async Task<int> SaveAsync(SimpleDocumentEntity simpleDocument)
    {
        var document = new SimpleDocument { Description = simpleDocument.Description, DocumentNum = simpleDocument.DocumentNum };

        this._dbContext.SimpleDocuments.Add(document);

        await this._dbContext.SaveChangesAsync();

        return simpleDocument.SerialId;
    }
}