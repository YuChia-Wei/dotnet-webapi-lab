using Wolverine;

namespace dotnetLab.UseCases.SimpleDocument.Commands;

/// <summary>
/// sample command
/// </summary>
public class InputSimpleDocumentCommand : ICommand
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