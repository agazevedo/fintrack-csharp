using Fintrack.API.Controllers;
using Fintrack.API.Data;
using Fintrack.API.DTOs.Categories;
using Fintrack.API.Models;
using Fintrack.API.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Fintrack.API.Tests.Unit.Controllers;

public class CategoriesControllerTests {
	private const int UserId = 1;
	private const int OtherUserId = 2;

	private readonly AppDbContext _context = TestHelpers.CreateInMemoryContext();
	private readonly CategoriesController _controller;

	public CategoriesControllerTests() {
		_controller = new CategoriesController(_context).WithUser(UserId);
	}

	[Fact]
	public async Task GetAll_ReturnsOnlyCurrentUserCategories() {
		_context.Categories.AddRange(
			new Category { Name = "Mercado", Type = "expense", UserId = UserId },
			new Category { Name = "Salário", Type = "income", UserId = OtherUserId }
		);
		await _context.SaveChangesAsync();

		var result = await _controller.GetAll();

		var categories = Assert.IsAssignableFrom<IEnumerable<Category>>(Assert.IsType<OkObjectResult>(result).Value);
		Assert.Equal("Mercado", Assert.Single(categories).Name);
	}

	[Fact]
	public async Task Create_NewCategory_ReturnsCreatedForCurrentUser() {
		var result = await _controller.Create(new CreateCategoryDto { Name = "Lazer", Type = "expense" });

		var category = Assert.IsType<Category>(Assert.IsType<CreatedResult>(result).Value);
		Assert.Equal(UserId, category.UserId);
		Assert.Equal("Lazer", category.Name);
	}

	[Fact]
	public async Task Create_DuplicateNameIgnoringCase_ReturnsBadRequest() {
		_context.Categories.Add(new Category { Name = "Lazer", Type = "expense", UserId = UserId });
		await _context.SaveChangesAsync();

		var result = await _controller.Create(new CreateCategoryDto { Name = "LAZER", Type = "expense" });

		Assert.IsType<BadRequestObjectResult>(result);
	}

	[Fact]
	public async Task Update_ExistingCategory_ReturnsUpdated() {
		var category = new Category { Name = "Lazer", Type = "expense", UserId = UserId };
		_context.Categories.Add(category);
		await _context.SaveChangesAsync();

		var result = await _controller.Update(category.Id, new UpdateCategoryDto { Name = "Viagem", Type = "expense" });

		Assert.Equal("Viagem", Assert.IsType<Category>(Assert.IsType<OkObjectResult>(result).Value).Name);
	}

	[Fact]
	public async Task Update_CategoryFromAnotherUser_ReturnsNotFound() {
		var category = new Category { Name = "Lazer", Type = "expense", UserId = OtherUserId };
		_context.Categories.Add(category);
		await _context.SaveChangesAsync();

		var result = await _controller.Update(category.Id, new UpdateCategoryDto { Name = "Hack", Type = "expense" });

		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task Delete_ExistingCategory_ReturnsNoContent() {
		var category = new Category { Name = "Lazer", Type = "expense", UserId = UserId };
		_context.Categories.Add(category);
		await _context.SaveChangesAsync();

		var result = await _controller.Delete(category.Id);

		Assert.IsType<NoContentResult>(result);
		Assert.Empty(_context.Categories);
	}

	[Fact]
	public async Task Delete_NonExistentCategory_ReturnsNotFound() {
		Assert.IsType<NotFoundResult>(await _controller.Delete(999));
	}
}
