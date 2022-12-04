using dotnet.WebApi.Repository.Interfaces;
using dotnet.WebApi.Service.Dtos;
using MediatR;

namespace dotnet.WebApi.Service.Queries;

public class SampleDataQueryHandler : IRequestHandler<SampleDataQuery, SampleDto>
{
    private readonly ISampleDataRepository _sampleDataRepository;

    /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
    public SampleDataQueryHandler(ISampleDataRepository sampleDataRepository)
    {
        this._sampleDataRepository = sampleDataRepository;
    }

    /// <summary>Handles a request</summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public async Task<SampleDto> Handle(SampleDataQuery request, CancellationToken cancellationToken)
    {
        var sampleData = await this._sampleDataRepository.GetAsync(request.SerialId);

        return new SampleDto
        {
            SerialId = sampleData.SerialId,
            Description = sampleData.Description,
            DocumentNum = sampleData.DocumentNum
        };
    }
}