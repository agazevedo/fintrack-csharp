using System.ComponentModel.DataAnnotations;

namespace Fintrack.API.Models;

public class User {
	public int Id { get; set; }

	public DateTime TsCreated { get; set; }

	public string Username { get; set; } = string.Empty;

	[MaxLength(255)]
	public string Password { get; set; } = string.Empty;

	public ICollection<Category> Categories { get; set; } = [];
	public ICollection<BudgetItem> BudgetItems { get; set; } = [];
}
