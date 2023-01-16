using dotnet.WebApi.Service.Dtos;

namespace dotnet.WebApi.Service.Queries;

public class SampleDataQuery : IRequest<SampleDto>
{
    /// <summary>
    /// 序號
    /// </summary>
    public int SerialId { get; set; }
}