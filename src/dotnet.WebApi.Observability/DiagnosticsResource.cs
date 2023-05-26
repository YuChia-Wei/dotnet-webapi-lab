namespace dotnet.WebApi.Observability;

public static class DiagnosticsResource
{
    /// <summary>
    /// 取得追蹤系統所定義的服務名稱，open telemetry 自動追蹤工具會使用此名稱所定義的 ActivitySource / Meter 來收集資料。
    /// 此名稱通常使用 OTEL_SERVICE_NAME 的環境參數來定義，若未定義，則使用系統進入點名稱，全小寫 & 分隔符號為 - 號
    /// </summary>
    /// <returns></returns>
    public static string Name()
    {
        return Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME") ??
               AppDomain.CurrentDomain.FriendlyName.ToLower().Replace('.', '-');
    }
}