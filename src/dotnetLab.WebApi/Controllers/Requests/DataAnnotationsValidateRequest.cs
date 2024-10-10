using System.ComponentModel.DataAnnotations;

namespace dotnetLab.WebApi.Controllers.Requests;

public class DataAnnotationsValidateRequest
{
    [MaxLength(10)]
    [MinLength(1)]
    public string SerialId { get; set; }
}