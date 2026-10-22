using dotnetLab.Application.SimpleDocument.Dtos;

namespace dotnetLab.Application.SimpleDocument.UseCases;

/// <summary>查詢文件的業務輸入。</summary>
public sealed record GetSimpleDocumentInput(int SerialId);
/// <summary>建立文件的業務輸入。</summary>
public sealed record CreateSimpleDocumentInput(string DocumentNum, string? Description);
/// <summary>更新文件敘述的業務輸入。</summary>
public sealed record UpdateSimpleDocumentDescriptionInput(int SerialId, string? Description);

/// <summary>查詢文件的應用程式邊界。</summary>
public interface IGetSimpleDocumentUseCase
{
    /// <summary>取得唯讀模型；找不到時回傳 null。</summary>
    Task<SimpleDocumentDto?> ExecuteAsync(GetSimpleDocumentInput input, CancellationToken cancellationToken);
}
/// <summary>建立文件的應用程式邊界。</summary>
public interface ICreateSimpleDocumentUseCase
{
    /// <summary>建立文件並取得儲存層指派的序號。</summary>
    Task<int> ExecuteAsync(CreateSimpleDocumentInput input, CancellationToken cancellationToken);
}
/// <summary>更新文件敘述的應用程式邊界。</summary>
public interface IUpdateSimpleDocumentDescriptionUseCase
{
    /// <summary>更新文件；找不到時回傳 false。</summary>
    Task<bool> ExecuteAsync(UpdateSimpleDocumentDescriptionInput input, CancellationToken cancellationToken);
}