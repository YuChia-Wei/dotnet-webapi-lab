using dotnet.WebApi.Repository.Db.SampleDb.Entities;
using dotnet.WebApi.Repository.Interfaces;

namespace dotnet.WebApi.Service.Commands;

/// <summary>
/// sample command handler
/// </summary>
public class InputSampleDataCommandHandler : IRequestHandler<InputSampleDataCommand, int>
{
    private readonly ISampleDataRepository _sampleDataRepository;

    public InputSampleDataCommandHandler(ISampleDataRepository sampleDataRepository)
    {
        this._sampleDataRepository = sampleDataRepository;
    }

    /// <summary>Handles a request</summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from the request</returns>
    public async ValueTask<int> Handle(InputSampleDataCommand request, CancellationToken cancellationToken)
    {
        var sampleData = new SampleData { DocumentNum = request.DocumentNum, Description = request.Description };
        return await this._sampleDataRepository.SaveAsync(sampleData);
    }
}