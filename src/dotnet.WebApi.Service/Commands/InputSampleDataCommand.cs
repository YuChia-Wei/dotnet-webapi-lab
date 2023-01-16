namespace dotnet.WebApi.Service.Commands;

/// <summary>
/// sample command
/// </summary>
public class InputSampleDataCommand : IRequest<int>
{
    /// <summary>
    /// serial Id
    /// </summary>
    public string DocumentNum { get; set; } = string.Empty;

    /// <summary>
    /// 敘述
    /// </summary>
    public string? Description { get; set; }
}