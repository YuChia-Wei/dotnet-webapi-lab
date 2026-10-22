using ArchPrecept.Contracts.Application;
using dotnetLab.Domain.SampleDoc;

namespace dotnetLab.Infrastructure.Repositories;

/// <summary>無資料庫的 scope 內示範儲存庫；未保存的正整數序號會產生原有範例資料。</summary>
internal sealed class MockDataRepository : IAggregateRepository<SimpleDocumentEntity, int>
{
    private readonly Dictionary<int, SimpleDocumentEntity> _documents = new();
    private int _nextId = 123;

    /// <inheritdoc />
    public Task<SimpleDocumentEntity?> FindByIdAsync(int serialId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (serialId <= 0) return Task.FromResult<SimpleDocumentEntity?>(null);
        if (!this._documents.TryGetValue(serialId, out var document))
        {
            document = SimpleDocumentEntity.Restore(serialId, $"no{serialId}", "for no db test");
            this._documents.Add(serialId, document);
        }
        return Task.FromResult<SimpleDocumentEntity?>(document);
    }

    /// <inheritdoc />
    public Task SaveAsync(SimpleDocumentEntity aggregate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (aggregate.Id == 0) aggregate.AssignIdentity(this._nextId++);
        this._documents[aggregate.Id] = aggregate;
        return Task.CompletedTask;
    }

}