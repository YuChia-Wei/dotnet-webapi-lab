namespace dotnetLab.WebApi.Controllers.ViewModels;

public class ErrorResultViewModel
{
    public string? ApiVersion { get; set; }
    public string RequestPath { get; set; }
    public ErrorInformation Error { get; set; }
}