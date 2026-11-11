using System.ComponentModel.DataAnnotations;

namespace Fintrack.API.DTOs.BudgetItems;

public class CreateBudgetItemDto {
	[Required]
	public int CategoryId { get; set; }

	[Required(ErrorMessage = "Informe a descrição do item.")]
	public string Description { get; set; } = string.Empty;

	[Range(typeof(decimal), "0.01", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true,
		ErrorMessage = "Informe um valor maior que zero.")]
	public decimal UnitValue { get; set; }

	[Range(1, int.MaxValue, ErrorMessage = "Informe uma quantidade de pelo menos 1.")]
	public int Quantity { get; set; }
}
