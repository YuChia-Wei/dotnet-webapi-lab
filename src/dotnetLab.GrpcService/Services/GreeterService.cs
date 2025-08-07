using dotnet.GrpcService;
using dotnetLab.Application.SimpleDocument.Queries;
using Grpc.Core;
using Wolverine;

namespace dotnetLab.GrpcService.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;

    private readonly IMessageBus _messageBus;

    public GreeterService(ILogger<GreeterService> logger, IMessageBus messageBus)
    {
        this._logger = logger;
        this._messageBus = messageBus;
    }

    public override async Task<SampleReply> SayHello(SampleQuery request, ServerCallContext context)
    {
        var sampleDataQuery = new SimpleDocQuery
        {
            SerialId = request.SerialId
        };

        var sampleDto = await this._messageBus.InvokeAsync<object>(sampleDataQuery);

        return new SampleReply
        {
            Description = sampleDto.ToString()
        };
    }
}