using ArchPrecept.Contracts.Application;
using Dapper;
using dotnetLab.Application.SimpleDocument.Dtos;
using dotnetLab.Application.SimpleDocument.Ports.Out;
using dotnetLab.Domain.SampleDoc;
using dotnetLab.Infrastructure.Enums;
using dotnetLab.Infrastructure.Factories;
using dotnetLab.Persistence.Metadata.SampleDb;
using dotnetLab.Persistence.Metadata.SampleDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace dotnetLab.Infrastructure.Repositories;

/// <summary>以 Dapper 查詢、EF Core 保存文件的外部儲存 adapter；需由主機設定連線。</summary>
internal sealed class SimpleDocumentRepository(IDbConnectionFactory connectionFactory, SampleDbContext dbContext)
    : IAggregateRepository<SimpleDocumentEntity, int>, ISimpleDocumentQueryRepository
{
    /// <inheritdoc />
    public async Task<SimpleDocumentEntity?> FindByIdAsync(int serialId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection(DbInstanceEnum.SamplePostgreSQL);
        var command = new CommandDefinition("select * from SimpleDocument where serialId = @serialId", new { serialId },
            cancellationToken: cancellationToken);
        var row = await connection.QueryFirstOrDefaultAsync<SimpleDocument>(command);
        return row is null ? null : SimpleDocumentEntity.Restore(row.SerialId, row.DocumentNum, row.Description);
    }

    /// <inheritdoc />
    public async Task SaveAsync(SimpleDocumentEntity aggregate, CancellationToken cancellationToken = default)
    {
        if (aggregate.Id == 0)
        {
            var row = new SimpleDocument { DocumentNum = aggregate.DocumentNum, Description = aggregate.Description };
            dbContext.SimpleDocuments.Add(row);
            await dbContext.SaveChangesAsync(cancellationToken);
            aggregate.AssignIdentity(row.SerialId);
            return;
        }
        var existing = await dbContext.SimpleDocuments.FindAsync(new object[] { aggregate.Id }, cancellationToken);
        if (existing is null) throw new DbUpdateConcurrencyException("要更新的文件已不存在。");
        existing.Description = aggregate.Description;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    async Task<SimpleDocumentDto?> ISimpleDocumentQueryRepository.FindByIdAsync(int serialId, CancellationToken cancellationToken)
    {
        var document = await this.FindByIdAsync(serialId, cancellationToken);
        return document is null ? null : new SimpleDocumentDto
        {
            SerialId = document.SerialId,
            DocumentNum = document.DocumentNum,
            Description = document.Description
        };
    }
}