namespace dotnetLab.WebApi.Controllers.Requests;

/// <summary>
/// sample command
/// </summary>
public class NewSimpleDocumentRequest
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