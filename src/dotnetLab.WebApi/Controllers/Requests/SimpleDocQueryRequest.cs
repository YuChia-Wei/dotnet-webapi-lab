namespace dotnetLab.WebApi.Controllers.Requests;

/// <summary>文件序號查詢參數。</summary>
public class SimpleDocQueryRequest
{
    /// <summary>
    /// 序號
    /// </summary>
    /// <example>2</example>
    public int SerialId { get; set; }
}