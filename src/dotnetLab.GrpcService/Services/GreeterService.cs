using dotnet.GrpcService;
using dotnetLab.UseCase.SimpleDocument.Queries;
using Grpc.Core;
using Mediator;

namespace dotnetLab.GrpcService.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;

    private readonly IMediator _mediator;

    public GreeterService(ILogger<GreeterService> logger, IMediator mediator)
    {
        this._logger = logger;
        this._mediator = mediator;
    }

    public override async Task<SampleReply> SayHello(SampleQuery request, ServerCallContext context)
    {
        var sampleDataQuery = new SimpleDocQuery { SerialId = request.SerialId };

        var sampleDto = await this._mediator.Send(sampleDataQuery);

        return new SampleReply { Description = sampleDto.Description };
    }
}