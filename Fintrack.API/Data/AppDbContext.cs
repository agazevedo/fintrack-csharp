using Microsoft.EntityFrameworkCore;
using Fintrack.API.Models;

namespace Fintrack.API.Data;

public class AppDbContext : DbContext {
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
	}

	public DbSet<User> Users => Set<User>();
	public DbSet<Category> Categories => Set<Category>();
	public DbSet<BudgetItem> BudgetItems => Set<BudgetItem>();
	public DbSet<Expense> Expenses => Set<Expense>();

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Category>()
			.HasIndex(c => new {
				c.UserId,
				c.Name
			})
			.IsUnique();

		modelBuilder.Entity<BudgetItem>()
			.HasIndex(b => new {
				b.UserId,
				b.CategoryId,
				b.Description
			})
			.IsUnique();
		
		modelBuilder.Entity<Expense>()
			.HasIndex(e => new {
				e.BudgetItemId,
				e.Date
			})
			.IsUnique();
	}
}
