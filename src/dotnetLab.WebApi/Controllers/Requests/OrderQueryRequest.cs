using System;

namespace dotnetLab.WebApi.Controllers.Requests;

/// <summary>
/// 取得訂單 Request
/// </summary>
public class OrderQueryRequest
{
    /// <summary>
    /// 訂單識別碼
    /// </summary>
    public Guid OrderId { get; set; }
}