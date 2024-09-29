namespace dotnetLab.WebApi.Controllers.Requests;

/// <summary>
/// simple document description update request 
/// </summary>
public class UpdateSimpleDocumentDescriptionRequest
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