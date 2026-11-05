using Fintrack.API.Controllers;
using Fintrack.API.Data;
using Fintrack.API.DTOs.BudgetItems;
using Fintrack.API.Models;
using Fintrack.API.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Fintrack.API.Tests.Unit.Controllers;

public class BudgetItemsControllerTests {
	private const int UserId = 1;
	private const int OtherUserId = 2;

	private readonly AppDbContext _context = TestHelpers.CreateInMemoryContext();
	private readonly BudgetItemsController _controller;
	private readonly Category _category;

	public BudgetItemsControllerTests() {
		_controller = new BudgetItemsController(_context).WithUser(UserId);

		_category = new Category { Name = "Casa", Type = "expense", UserId = UserId };
		_context.Categories.Add(_category);
		_context.SaveChanges();
	}

	private CreateBudgetItemDto ValidDto(int? categoryId = null) => new() {
		CategoryId = categoryId ?? _category.Id,
		Description = "Aluguel",
		UnitValue = 1500m,
		Quantity = 1
	};

	[Fact]
	public async Task Create_WithOwnCategory_ReturnsCreated() {
		var result = await _controller.Create(ValidDto());

		var item = Assert.IsType<BudgetItem>(Assert.IsType<CreatedResult>(result).Value);
		Assert.Equal(UserId, item.UserId);
		Assert.Equal(1500m, item.BudgetTotal);
	}

	[Fact]
	public async Task Create_WithCategoryFromAnotherUser_ReturnsBadRequest() {
		var foreign = new Category { Name = "Outra", Type = "expense", UserId = OtherUserId };
		_context.Categories.Add(foreign);
		await _context.SaveChangesAsync();

		Assert.IsType<BadRequestObjectResult>(await _controller.Create(ValidDto(foreign.Id)));
	}

	[Fact]
	public async Task GetAll_ReturnsOnlyCurrentUserItems() {
		_context.BudgetItems.AddRange(
			new BudgetItem { Description = "Meu", CategoryId = _category.Id, UserId = UserId },
			new BudgetItem { Description = "Dele", CategoryId = _category.Id, UserId = OtherUserId }
		);
		await _context.SaveChangesAsync();

		var result = await _controller.GetAll();

		var items = Assert.IsAssignableFrom<IEnumerable<BudgetItem>>(Assert.IsType<OkObjectResult>(result).Value);
		Assert.Equal("Meu", Assert.Single(items).Description);
	}

	[Fact]
	public async Task Update_ExistingItem_ReturnsUpdatedAndSetsUpdatedAt() {
		var item = new BudgetItem { Description = "Aluguel", CategoryId = _category.Id, UserId = UserId, UnitValue = 1m, Quantity = 1 };
		_context.BudgetItems.Add(item);
		await _context.SaveChangesAsync();

		var result = await _controller.Update(item.Id, ValidDto());

		var updated = Assert.IsType<BudgetItem>(Assert.IsType<OkObjectResult>(result).Value);
		Assert.Equal(1500m, updated.UnitValue);
		Assert.NotNull(updated.UpdatedAt);
	}

	[Fact]
	public async Task Update_NonExistentItem_ReturnsNotFound() {
		Assert.IsType<NotFoundResult>(await _controller.Update(999, ValidDto()));
	}

	[Fact]
	public async Task Delete_ExistingItem_ReturnsNoContent() {
		var item = new BudgetItem { Description = "Aluguel", CategoryId = _category.Id, UserId = UserId };
		_context.BudgetItems.Add(item);
		await _context.SaveChangesAsync();

		Assert.IsType<NoContentResult>(await _controller.Delete(item.Id));
		Assert.Empty(_context.BudgetItems);
	}

	[Fact]
	public async Task Delete_ItemFromAnotherUser_ReturnsNotFound() {
		var item = new BudgetItem { Description = "Dele", CategoryId = _category.Id, UserId = OtherUserId };
		_context.BudgetItems.Add(item);
		await _context.SaveChangesAsync();

		Assert.IsType<NotFoundResult>(await _controller.Delete(item.Id));
	}
}
