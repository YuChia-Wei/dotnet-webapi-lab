using dotnet.GrpcService;
using dotnetLab.Application.SimpleDocument.UseCases;
using Grpc.Core;

namespace dotnetLab.GrpcService.Services;

/// <summary>以文件查詢 Use Case 實作既有 gRPC 協定。</summary>
public sealed class GreeterService(IGetSimpleDocumentUseCase useCase) : Greeter.GreeterBase
{
    /// <summary>回傳文件敘述，並傳遞 gRPC 取消權杖。</summary>
    public override async Task<SampleReply> SayHello(SampleQuery request, ServerCallContext context)
    {
        var dto = await useCase.ExecuteAsync(new GetSimpleDocumentInput(request.SerialId), context.CancellationToken);
        return dto is null
            ? throw new RpcException(new Status(StatusCode.NotFound, "Document not found."))
            : new SampleReply { Description = dto.Description ?? string.Empty };
    }
}