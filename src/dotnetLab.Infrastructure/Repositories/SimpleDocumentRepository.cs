using System.Data;
using Dapper;
using dotnetLab.Application.SimpleDocument.Ports.Out;
using dotnetLab.Domain.SampleDoc;
using dotnetLab.Infrastructure.Enums;
using dotnetLab.Persistence.Metadata.SampleDb;
using dotnetLab.Persistence.Metadata.SampleDb.Entities;

namespace dotnetLab.Infrastructure.Repositories;

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
        var document = new SimpleDocument
        {
            Description = simpleDocument.Description,
            DocumentNum = simpleDocument.DocumentNum
        };

        this._dbContext.SimpleDocuments.Add(document);

        await this._dbContext.SaveChangesAsync();

        return simpleDocument.SerialId;
    }

    /// <summary>
    /// 儲存文件
    /// </summary>
    /// <param name="sampleTable"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(SimpleDocumentEntity sampleTable)
    {
        throw new NotImplementedException();
    }
}