using ArchPrecept.Contracts.Application;
using ArchPrecept.Contracts.Domain;
using dotnetLab.Application.Products.UseCases;
using dotnetLab.Application.SimpleDocument.Dtos;
using dotnetLab.Application.SimpleDocument.Ports.Out;
using dotnetLab.Application.SimpleDocument.UseCases;
using dotnetLab.Domain.Products;
using dotnetLab.Domain.SampleDoc;
using dotnetLab.Domain.SampleDoc.Events;
using dotnetLab.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace dotnetLab.Tests;

/// <summary>驗證查無資料、取消、保存及事件分派的業務順序。</summary>
public sealed class UseCaseBehaviorTests
{
    /// <summary>查不到商品時回傳 null，找到時保留既有公開欄位。</summary>
    [Fact]
    public async Task Product_query_preserves_result_shape_and_handles_missing_dataAsync()
    {
        var repository = new TestRepository<Product, Guid>();
        var useCase = new GetProductUseCase(repository);
        var id = Guid.NewGuid();
        Assert.Null(await useCase.ExecuteAsync(new GetProductInput(id), CancellationToken.None));
        repository.Value = new Product(id, "Milk", 100m, 1m, "10x10x10", true);
        var dto = await useCase.ExecuteAsync(new GetProductInput(id), CancellationToken.None);
        Assert.NotNull(dto);
        Assert.Equal(id, dto.Id);
        Assert.Equal("Milk", dto.Name);
        Assert.Equal(100m, dto.UnitPrice);
        Assert.Equal("10x10x10", dto.Dimensions);
        Assert.True(dto.RequiresRefrigeration);
    }

    /// <summary>商品查詢必須將取消權杖交給儲存庫。</summary>
    [Fact]
    public async Task Product_query_propagates_cancellationAsync()
    {
        var repository = new TestRepository<Product, Guid>();
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            new GetProductUseCase(repository).ExecuteAsync(new GetProductInput(Guid.NewGuid()), cts.Token));
        Assert.Equal(cts.Token, repository.LastToken);
    }

    /// <summary>文件查無資料時不發生空值解參考。</summary>
    [Fact]
    public async Task Missing_document_query_returns_nullAsync()
    {
        var useCase = new GetSimpleDocumentUseCase(new EmptyDocumentQueries());
        Assert.Null(await useCase.ExecuteAsync(new GetSimpleDocumentInput(1), CancellationToken.None));
    }

    /// <summary>建立文件取得儲存層實際產生的識別碼。</summary>
    [Fact]
    public async Task Create_document_returns_persisted_identityAsync()
    {
        var repository = new TestRepository<SimpleDocumentEntity, int>();
        var id = await new CreateSimpleDocumentUseCase(repository).ExecuteAsync(new CreateSimpleDocumentInput("DOC-1", "text"), CancellationToken.None);
        Assert.Equal(123, id);
        Assert.Equal("DOC-1", repository.Value!.DocumentNum);
    }

    /// <summary>不存在的文件不保存也不分派事件。</summary>
    [Fact]
    public async Task Missing_document_update_returns_falseAsync()
    {
        var repository = new TestRepository<SimpleDocumentEntity, int>();
        var dispatcher = new RecordingDispatcher(repository.Steps);
        var result = await new UpdateSimpleDocumentDescriptionUseCase(repository, dispatcher)
            .ExecuteAsync(new UpdateSimpleDocumentDescriptionInput(1, "new"), CancellationToken.None);
        Assert.False(result);
        Assert.Empty(repository.Steps);
    }

    /// <summary>保存成功後才分派事件，完成後清除待處理集合。</summary>
    [Fact]
    public async Task Update_saves_before_dispatch_and_clears_completed_eventsAsync()
    {
        var repository = new TestRepository<SimpleDocumentEntity, int> { Value = SimpleDocumentEntity.Restore(1, "DOC-1", "old") };
        var dispatcher = new RecordingDispatcher(repository.Steps);
        var result = await new UpdateSimpleDocumentDescriptionUseCase(repository, dispatcher)
            .ExecuteAsync(new UpdateSimpleDocumentDescriptionInput(1, "new"), CancellationToken.None);
        Assert.True(result);
        Assert.Equal(new[] { "save", "dispatch" }, repository.Steps);
        var domainEvent = Assert.IsType<SimpleDocumentDescriptionUpdatedEvent>(Assert.Single(dispatcher.Events));
        Assert.Equal("old", domainEvent.OldDescription);
        Assert.Equal("new", domainEvent.NewDescription);
        Assert.Empty(repository.Value.DomainEvents);
    }

    /// <summary>保存失敗時不能先分派事件。</summary>
    [Fact]
    public async Task Save_failure_does_not_dispatch_eventsAsync()
    {
        var repository = new TestRepository<SimpleDocumentEntity, int>
        {
            Value = SimpleDocumentEntity.Restore(1, "DOC-1", "old"),
            FailSave = true
        };
        var dispatcher = new RecordingDispatcher(repository.Steps);
        await Assert.ThrowsAsync<InvalidOperationException>(() => new UpdateSimpleDocumentDescriptionUseCase(repository, dispatcher)
            .ExecuteAsync(new UpdateSimpleDocumentDescriptionInput(1, "new"), CancellationToken.None));
        Assert.Empty(dispatcher.Events);
        Assert.Single(repository.Value.DomainEvents);
    }

    /// <summary>分派失敗時保存已完成，但事件仍保留供呼叫端處理失敗。</summary>
    [Fact]
    public async Task Dispatch_failure_preserves_pending_events_after_saveAsync()
    {
        var repository = new TestRepository<SimpleDocumentEntity, int>
        {
            Value = SimpleDocumentEntity.Restore(1, "DOC-1", "old")
        };
        var dispatcher = new RecordingDispatcher(repository.Steps) { FailDispatch = true };
        await Assert.ThrowsAsync<InvalidOperationException>(() => new UpdateSimpleDocumentDescriptionUseCase(repository, dispatcher)
            .ExecuteAsync(new UpdateSimpleDocumentDescriptionInput(1, "new"), CancellationToken.None));
        Assert.Equal(new[] { "save", "dispatch" }, repository.Steps);
        Assert.Single(repository.Value.DomainEvents);
        Assert.Equal("new", repository.Value.Description);
    }

    /// <summary>分離查詢與聚合 adapter 後，同一 scope 的查詢仍能讀到已保存內容並傳遞取消。</summary>
    [Fact]
    public async Task Document_query_uses_same_scoped_store_and_preserves_cancellationAsync()
    {
        using var provider = new ServiceCollection().AddDataSource().BuildServiceProvider();
        using var scope = provider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAggregateRepository<SimpleDocumentEntity, int>>();
        var queries = scope.ServiceProvider.GetRequiredService<ISimpleDocumentQueryRepository>();
        var document = new SimpleDocumentEntity("SAVED", "initial");
        await repository.SaveAsync(document, CancellationToken.None);
        var snapshot = await queries.FindByIdAsync(document.Id, CancellationToken.None);
        Assert.NotNull(snapshot);
        Assert.Equal("SAVED", snapshot.DocumentNum);
        document.UpdateDescription("updated");
        await repository.SaveAsync(document, CancellationToken.None);
        Assert.Equal("initial", snapshot.Description);
        Assert.Equal("updated", (await queries.FindByIdAsync(document.Id, CancellationToken.None))!.Description);
        Assert.Null(await queries.FindByIdAsync(0, CancellationToken.None));
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => queries.FindByIdAsync(document.Id, cts.Token));
    }

    private sealed class EmptyDocumentQueries : ISimpleDocumentQueryRepository
    {
        public Task<SimpleDocumentDto?> FindByIdAsync(int serialId, CancellationToken cancellationToken = default) => Task.FromResult<SimpleDocumentDto?>(null);
    }

    private sealed class RecordingDispatcher(List<string> steps) : IDomainEventDispatcher
    {
        public bool FailDispatch { get; init; }
        public List<IDomainEvent> Events { get; } = new();
        public Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            steps.Add("dispatch");
            if (this.FailDispatch) throw new InvalidOperationException("Dispatch unavailable.");
            this.Events.AddRange(events);
            return Task.CompletedTask;
        }
    }

    private sealed class TestRepository<TAggregate, TId> : IAggregateRepository<TAggregate, TId>
        where TAggregate : class, IAggregateRoot<TId> where TId : notnull
    {
        public TAggregate? Value { get; set; }
        public bool FailSave { get; init; }
        public List<string> Steps { get; } = new();
        public CancellationToken LastToken { get; private set; }
        public Task<TAggregate?> FindByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            this.LastToken = cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(this.Value);
        }
        public Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (this.FailSave) throw new InvalidOperationException("Storage unavailable.");
            if (aggregate is SimpleDocumentEntity { Id: 0 } document) document.AssignIdentity(123);
            this.Value = aggregate;
            this.Steps.Add("save");
            return Task.CompletedTask;
        }
    }
}