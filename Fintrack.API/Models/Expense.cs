namespace Fintrack.API.Models;

public class Expense {
	public int Id { get; set; }

	public int BudgetItemId { get; set; }

	public int Quantity { get; set; }

	public DateOnly Date { get; set; }

	public decimal UnitValue { get; set; }

	public BudgetItem BudgetItem { get; set; } = null!;

	public decimal Total => Quantity * UnitValue;
}
