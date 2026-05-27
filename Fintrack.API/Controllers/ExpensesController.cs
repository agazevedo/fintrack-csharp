using System.Security.Claims;
using Fintrack.API.Data;
using Fintrack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.API.Controllers;

[ApiController]
[Route("api/expenses")]
[Authorize]
public class ExpensesController : ControllerBase {
	private readonly AppDbContext _context;

	public ExpensesController(AppDbContext context) {
		_context = context;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll() {
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

		var expenses = await _context.Expenses
			.Include(e => e.BudgetItem)
			.ThenInclude(b => b.Category)
			.Where(e => e.BudgetItem.UserId == userId)
			.ToListAsync();

		return Ok(expenses);
	}

	[HttpPost]
	public async Task<IActionResult> Create(Expense expense) {
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

		var budgetItem = await _context.BudgetItems
			.FirstOrDefaultAsync(b =>
				b.Id == expense.BudgetItemId &&
				b.UserId == userId
			);

		if (budgetItem == null) {
			return BadRequest(new
			{
				message = "Invalid budget item"
			});
		}

		_context.Expenses.Add(expense);

		await _context.SaveChangesAsync();

		return Created("", expense);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(int id, Expense updatedExpense) {
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

		var expense = await _context.Expenses
			.Include(e => e.BudgetItem)
			.FirstOrDefaultAsync(e =>
				e.Id == id &&
				e.BudgetItem.UserId == userId
			);

		if (expense == null) {
			return NotFound();
		}

		expense.UnitValue = updatedExpense.UnitValue;
		expense.Quantity = updatedExpense.Quantity;
		expense.Date = updatedExpense.Date;

		await _context.SaveChangesAsync();

		return Ok(expense);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id) {
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

		var expense = await _context.Expenses
			.Include(e => e.BudgetItem)
			.FirstOrDefaultAsync(e =>
				e.Id == id &&
				e.BudgetItem.UserId == userId
			);

		if (expense == null) {
			return NotFound();
		}

		_context.Expenses.Remove(expense);

		await _context.SaveChangesAsync();

		return NoContent();
	}
}
