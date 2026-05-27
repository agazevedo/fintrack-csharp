using System.Text.Json.Serialization;

namespace Fintrack.API.Models;

public class BudgetItem {
	public int Id { get; set; }

	public int UserId { get; set; }

	public int CategoryId { get; set; }

	public string Description { get; set; } = string.Empty;

	public decimal UnitValue { get; set; }

	public int Quantity { get; set; }

	public DateTime CreatedAt { get; set; }

	public DateTime? UpdatedAt { get; set; }

	public User User { get; set; } = null!;

	public Category Category { get; set; } = null!;

	[JsonIgnore]
	public ICollection<Expense> Expenses { get; set; } = new List<Expense>();

	public decimal BudgetTotal => UnitValue * Quantity;

	public decimal Spent => Expenses.Sum(e => e.Total);

	public decimal Remaining => BudgetTotal - Spent;
}
