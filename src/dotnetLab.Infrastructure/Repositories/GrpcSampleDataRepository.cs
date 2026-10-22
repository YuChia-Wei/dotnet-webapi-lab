using dotnetLab.Application.SimpleDocument.Dtos;
using dotnetLab.Application.SimpleDocument.Ports.Out;
using Grpc.Net.Client;
using GrpcGreeterClient;

namespace dotnetLab.Infrastructure.Repositories;

/// <summary>既有 gRPC 協定僅提供查詢，因此只實作唯讀 Port，不宣告不支援的寫入操作。</summary>
internal sealed class GrpcSampleDataRepository : ISimpleDocumentQueryRepository
{
    /// <inheritdoc />
    public async Task<SimpleDocumentDto?> FindByIdAsync(int serialId, CancellationToken cancellationToken = default)
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7130");
        var client = new Greeter.GreeterClient(channel);
        var result = await client.SayHelloAsync(new SampleQuery { SerialId = serialId }, cancellationToken: cancellationToken);
        return new SimpleDocumentDto { SerialId = serialId, DocumentNum = $"no{serialId}", Description = result.Description };
    }
}