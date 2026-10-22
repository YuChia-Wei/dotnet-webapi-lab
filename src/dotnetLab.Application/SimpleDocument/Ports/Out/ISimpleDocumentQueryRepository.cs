using ArchPrecept.Contracts.Application;
using dotnetLab.Application.SimpleDocument.Dtos;

namespace dotnetLab.Application.SimpleDocument.Ports.Out;

/// <summary>提供文件唯讀模型，不暴露可修改的聚合根。</summary>
public interface ISimpleDocumentQueryRepository : IQueryRepository
{
    /// <summary>依序號取得文件唯讀資料。</summary>
    Task<SimpleDocumentDto?> FindByIdAsync(int serialId, CancellationToken cancellationToken = default);
}