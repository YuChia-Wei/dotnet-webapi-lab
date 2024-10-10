using System.ComponentModel.DataAnnotations;

namespace dotnetLab.WebApi.Controllers.Requests;

public class TypeValidateRequest
{
    [Required]
    public int SerialId { get; set; }

    [Required]
    [StringLength(50)]
    public string Description { get; set; }

    [Required]
    [Range(0,100)]
    public int DocumentNum { get; set; }
}