namespace dotnetLab.UseCase.SimpleDocument.Commands;

/// <summary>
/// sample command
/// </summary>
public class InputSimpleDocumentCommand : IRequest<int>
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