using System.ComponentModel.DataAnnotations;

namespace Fintrack.API.DTOs.BudgetItems;

public class CreateBudgetItemDto {
	[Required]
	public int CategoryId { get; set; }

	[Required]
	public string Description { get; set; } = string.Empty;

	[Required]
	public decimal UnitValue { get; set; }

	[Required]
	public int Quantity { get; set; }
}
