namespace dotnetLab.UseCase.SimpleDocument.Commands;

/// <summary>
/// sample command
/// </summary>
public class UpdateSimpleDocumentDescriptionCommand : IRequest<bool>
{
    /// <summary>
    /// 序號
    /// </summary>
    public int SerialId { get; set; }

    /// <summary>
    /// 敘述
    /// </summary>
    public string? Description { get; set; }
}