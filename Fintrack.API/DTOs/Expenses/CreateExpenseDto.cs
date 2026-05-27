using System.ComponentModel.DataAnnotations;

namespace Fintrack.API.DTOs.Expenses;

public class CreateExpenseDto {
	[Required]
	public int BudgetItemId { get; set; }

	[Required]
	public decimal UnitValue { get; set; }

	[Required]
	public int Quantity { get; set; }

	[Required]
	public DateOnly Date { get; set; }
}
