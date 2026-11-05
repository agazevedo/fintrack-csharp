using Fintrack.API.Controllers;
using Fintrack.API.Data;
using Fintrack.API.DTOs.Expenses;
using Fintrack.API.Models;
using Fintrack.API.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Fintrack.API.Tests.Unit.Controllers;

public class ExpensesControllerTests {
	private const int UserId = 1;
	private const int OtherUserId = 2;

	private readonly AppDbContext _context = TestHelpers.CreateInMemoryContext();
	private readonly ExpensesController _controller;
	private readonly BudgetItem _ownItem;
	private readonly BudgetItem _foreignItem;

	public ExpensesControllerTests() {
		_controller = new ExpensesController(_context).WithUser(UserId);

		var category = new Category { Name = "Casa", Type = "expense", UserId = UserId };
		_ownItem = new BudgetItem { Description = "Luz", Category = category, UserId = UserId, UnitValue = 200m, Quantity = 1 };
		_foreignItem = new BudgetItem { Description = "Água", Category = category, UserId = OtherUserId, UnitValue = 100m, Quantity = 1 };
		_context.BudgetItems.AddRange(_ownItem, _foreignItem);
		_context.SaveChanges();
	}

	private CreateExpenseDto Dto(int budgetItemId) => new() {
		BudgetItemId = budgetItemId,
		UnitValue = 80m,
		Quantity = 2,
		Date = new DateOnly(2026, 11, 1)
	};

	[Fact]
	public async Task Create_ForOwnBudgetItem_ReturnsCreated() {
		var result = await _controller.Create(Dto(_ownItem.Id));

		var expense = Assert.IsType<Expense>(Assert.IsType<CreatedResult>(result).Value);
		Assert.Equal(160m, expense.Total);
	}

	[Fact]
	public async Task Create_ForBudgetItemFromAnotherUser_ReturnsBadRequest() {
		Assert.IsType<BadRequestObjectResult>(await _controller.Create(Dto(_foreignItem.Id)));
	}

	[Fact]
	public async Task GetAll_ReturnsOnlyExpensesOfCurrentUserItems() {
		_context.Expenses.AddRange(
			new Expense { BudgetItemId = _ownItem.Id, UnitValue = 10m, Quantity = 1, Date = new DateOnly(2026, 11, 1) },
			new Expense { BudgetItemId = _foreignItem.Id, UnitValue = 20m, Quantity = 1, Date = new DateOnly(2026, 11, 1) }
		);
		await _context.SaveChangesAsync();

		var result = await _controller.GetAll();

		var expenses = Assert.IsAssignableFrom<IEnumerable<Expense>>(Assert.IsType<OkObjectResult>(result).Value);
		Assert.Equal(_ownItem.Id, Assert.Single(expenses).BudgetItemId);
	}

	[Fact]
	public async Task Update_ExistingExpense_ReturnsUpdated() {
		var expense = new Expense { BudgetItemId = _ownItem.Id, UnitValue = 10m, Quantity = 1, Date = new DateOnly(2026, 10, 1) };
		_context.Expenses.Add(expense);
		await _context.SaveChangesAsync();

		var result = await _controller.Update(expense.Id, Dto(_ownItem.Id));

		var updated = Assert.IsType<Expense>(Assert.IsType<OkObjectResult>(result).Value);
		Assert.Equal(80m, updated.UnitValue);
		Assert.Equal(new DateOnly(2026, 11, 1), updated.Date);
	}

	[Fact]
	public async Task Update_ExpenseFromAnotherUser_ReturnsNotFound() {
		var expense = new Expense { BudgetItemId = _foreignItem.Id, UnitValue = 10m, Quantity = 1, Date = new DateOnly(2026, 10, 1) };
		_context.Expenses.Add(expense);
		await _context.SaveChangesAsync();

		Assert.IsType<NotFoundResult>(await _controller.Update(expense.Id, Dto(_foreignItem.Id)));
	}

	[Fact]
	public async Task Delete_ExistingExpense_ReturnsNoContent() {
		var expense = new Expense { BudgetItemId = _ownItem.Id, UnitValue = 10m, Quantity = 1, Date = new DateOnly(2026, 10, 1) };
		_context.Expenses.Add(expense);
		await _context.SaveChangesAsync();

		Assert.IsType<NoContentResult>(await _controller.Delete(expense.Id));
		Assert.Empty(_context.Expenses);
	}

	[Fact]
	public async Task Delete_NonExistentExpense_ReturnsNotFound() {
		Assert.IsType<NotFoundResult>(await _controller.Delete(999));
	}
}
