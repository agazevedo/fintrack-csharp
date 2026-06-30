using System.ComponentModel.DataAnnotations;

namespace Fintrack.API.DTOs.Categories;

public class UpdateCategoryDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = string.Empty;
}
