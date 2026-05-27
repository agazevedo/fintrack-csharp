using System.Security.Claims;
using Fintrack.API.Data;
using Fintrack.API.Models;
using Fintrack.API.DTOs.BudgetItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.API.Controllers;

[ApiController]
[Route("api/budget-items")]
[Authorize]
public class BudgetItemsController : ControllerBase {
	private readonly AppDbContext _context;

	public BudgetItemsController(AppDbContext context) {
		_context = context;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll(){
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

		var items = await _context.BudgetItems
			.Include(b => b.Category)
			.Include(b => b.Expenses)
			.Where(b => b.UserId == userId)
			.ToListAsync();

		return Ok(items);
	}

	[HttpPost]
	public async Task<IActionResult> Create(CreateBudgetItemDto dto) {
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

		var categoryExists = await _context.Categories
			.AnyAsync(c =>
				c.Id == dto.CategoryId &&
				c.UserId == userId
			);

		if (!categoryExists) {
			return BadRequest(new {message = "Invalid category"});
		}

		var item = new BudgetItem {
			Description = dto.Description,
			UnitValue = dto.UnitValue,
			Quantity = dto.Quantity,
			CategoryId = dto.CategoryId,
			UserId = userId,
			CreatedAt = DateTime.UtcNow
		};

		_context.BudgetItems.Add(item);

		await _context.SaveChangesAsync();

		return Created("", item);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(int id, CreateBudgetItemDto dto) {
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

		var item = await _context.BudgetItems
			.FirstOrDefaultAsync(b =>
				b.Id == id &&
				b.UserId == userId
			);

		if (item == null) {
			return NotFound();
		}

		item.Description = dto.Description;
		item.UnitValue = dto.UnitValue;
		item.Quantity = dto.Quantity;
		item.CategoryId = dto.CategoryId;
		item.UpdatedAt = DateTime.UtcNow;

		await _context.SaveChangesAsync();

		return Ok(item);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id) {
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

		var item = await _context.BudgetItems
			.FirstOrDefaultAsync(b =>
				b.Id == id &&
				b.UserId == userId
			);

		if (item == null) {
			return NotFound();
		}

		_context.BudgetItems.Remove(item);

		await _context.SaveChangesAsync();

		return NoContent();
	}
}
