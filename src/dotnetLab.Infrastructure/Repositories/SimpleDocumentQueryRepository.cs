using ArchPrecept.Contracts.Application;
using dotnetLab.Application.SimpleDocument.Dtos;
using dotnetLab.Application.SimpleDocument.Ports.Out;
using dotnetLab.Domain.SampleDoc;

namespace dotnetLab.Infrastructure.Repositories;

/// <summary>從同一個 scoped 聚合儲存庫取得文件，僅透過查詢 Port 回傳 DTO。</summary>
internal sealed class SimpleDocumentQueryRepository(IAggregateRepository<SimpleDocumentEntity, int> repository)
    : ISimpleDocumentQueryRepository
{
    /// <inheritdoc />
    public async Task<SimpleDocumentDto?> FindByIdAsync(int serialId, CancellationToken cancellationToken = default)
    {
        var document = await repository.FindByIdAsync(serialId, cancellationToken);
        return document is null ? null : new SimpleDocumentDto
        {
            SerialId = document.SerialId,
            DocumentNum = document.DocumentNum,
            Description = document.Description
        };
    }
}