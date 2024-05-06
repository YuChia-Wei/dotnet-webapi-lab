using dotnetLab.UseCase.SimpleDocument.Dtos;

namespace dotnetLab.UseCase.SimpleDocument.Queries;

public class SimpleDocQuery : IRequest<SimpleDocumentDto>
{
    /// <summary>
    /// 序號
    /// </summary>
    public int SerialId { get; set; }
}