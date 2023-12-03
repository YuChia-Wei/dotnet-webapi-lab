using dotnet.WebApi.Database.SampleDb.Entities;
using dotnet.WebApi.Repository.Interfaces;
using Grpc.Net.Client;
using GrpcGreeterClient;

namespace dotnet.WebApi.Repository.Implements;

public class GrpcSampleDataRepository : ISampleDataRepository
{
    public async Task<SampleTable> GetAsync(int serialId)
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7130");
        var client = new Greeter.GreeterClient(channel);
        var result = await client.SayHelloAsync(new SampleQuery { SerialId = serialId });
        var sampleTable = new SampleTable()
        {
            SerialId = serialId,
            Description = result.Description
        };
        return sampleTable;
    }

    public async Task<int> SaveAsync(SampleTable sampleTable)
    {
        throw new NotImplementedException();
    }
}