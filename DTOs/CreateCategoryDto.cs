using System.ComponentModel.DataAnnotations;
namespace Inventory.Api.DTOs;

public class CreateCategoryDto
{
    // [Required] ya rechaza null y cadenas vacias (AllowEmptyString es false por defecto)
    [Required]
    [StringLength(100)] // ajusta al largo real de la columna en tu base de datos
    public string Name { get; set; } = string.Empty;
    
    // Opcional: el "?" la marca como nulable y el binder no la exige
    [StringLength(500)]
    public string? Description { get; set; }
}