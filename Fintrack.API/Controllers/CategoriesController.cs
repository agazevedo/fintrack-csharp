using System.Security.Claims;
using Fintrack.API.Data;
using Fintrack.API.Models;
using Fintrack.API.DTOs.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.API.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController : ControllerBase {
	private readonly AppDbContext _context;

	public CategoriesController(AppDbContext context) {
		_context = context;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll() {
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

		var categories = await _context.Categories.Where(c => c.UserId == userId).ToListAsync();

		return Ok(categories);
	}

	[HttpPost]
	public async Task<IActionResult> Create(CreateCategoryDto dto) {
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

		var exists = await _context.Categories.AnyAsync(c =>
			c.UserId == userId &&
			c.Name.ToLower() == dto.Name.ToLower()
		);

		if (exists) {
			return BadRequest(new {message = "Category already exists"});
		}

		var category = new Category {
			Name = dto.Name,
			Type = dto.Type,
			UserId = userId
		};

		_context.Categories.Add(category);

		await _context.SaveChangesAsync();

		return Created("", category);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(int id, Category updatedCategory) {
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

		var category = await _context.Categories
			.FirstOrDefaultAsync(c =>
				c.Id == id &&
				c.UserId == userId
			);

		if (category == null) {
			return NotFound();
		}

		category.Name = updatedCategory.Name;
		category.Type = updatedCategory.Type;

		await _context.SaveChangesAsync();

		return Ok(category);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id) {
		var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

		var category = await _context.Categories
			.FirstOrDefaultAsync(c =>
				c.Id == id &&
				c.UserId == userId
			);

		if (category == null) {
			return NotFound();
		}

		_context.Categories.Remove(category);

		await _context.SaveChangesAsync();

		return NoContent();
	}
}
