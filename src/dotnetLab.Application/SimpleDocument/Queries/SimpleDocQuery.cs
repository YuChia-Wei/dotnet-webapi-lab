using dotnetLab.Application.SimpleDocument.Dtos;

namespace dotnetLab.Application.SimpleDocument.Queries;

public class SimpleDocQuery : IRequest<SimpleDocumentDto>
{
    /// <summary>
    /// 序號
    /// </summary>
    public int SerialId { get; set; }
}