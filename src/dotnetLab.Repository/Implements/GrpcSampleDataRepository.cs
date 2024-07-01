using dotnetLab.DomainEntity;
using dotnetLab.UseCase.SimpleDocument.Ports.Out;
using Grpc.Net.Client;
using GrpcGreeterClient;

namespace dotnetLab.Repository.Implements;

public class GrpcSampleDataRepository : ISimpleDocumentRepository
{
    public async Task<SimpleDocumentEntity?> GetAsync(int serialId)
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7130");
        var client = new Greeter.GreeterClient(channel);
        var result = await client.SayHelloAsync(new SampleQuery { SerialId = serialId });
        var sampleTable = new SimpleDocumentEntity { SerialId = serialId, Description = result.Description };
        return sampleTable;
    }

    public async Task<int> SaveAsync(SimpleDocumentEntity simpleDocument)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 儲存文件
    /// </summary>
    /// <param name="sampleTable"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(SimpleDocumentEntity sampleTable)
    {
        throw new NotImplementedException();
    }
}