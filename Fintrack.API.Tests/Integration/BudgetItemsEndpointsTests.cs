using System.Net;
using System.Net.Http.Json;

namespace Fintrack.API.Tests.Integration;

public class BudgetItemsEndpointsTests : IntegrationTestBase {
	public BudgetItemsEndpointsTests(FintrackApiFactory factory) : base(factory) { }

	[Fact]
	public async Task GetAll_WithoutToken_ReturnsUnauthorized() {
		var response = await Factory.CreateClient().GetAsync("/api/budget-items");

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task Create_ThenGetAll_ReturnsItemWithCategoryAndTotals() {
		var client = await CreateAuthenticatedClientAsync();
		var category = await CreateCategoryAsync(client);

		var response = await client.PostAsJsonAsync("/api/budget-items", new {
			categoryId = category.Id,
			description = "Aluguel",
			unitValue = 1000m,
			quantity = 2
		});
		Assert.Equal(HttpStatusCode.Created, response.StatusCode);

		var items = await client.GetFromJsonAsync<List<BudgetItemResponse>>("/api/budget-items");
		var item = Assert.Single(items!);
		Assert.Equal("Aluguel", item.Description);
		Assert.Equal(2000m, item.BudgetTotal);
		Assert.Equal(0m, item.Spent);
		Assert.Equal(2000m, item.Remaining);
		Assert.Equal(category.Name, item.Category!.Name);
	}

	[Fact]
	public async Task Create_WithInvalidCategory_ReturnsBadRequest() {
		var client = await CreateAuthenticatedClientAsync();

		var response = await client.PostAsJsonAsync("/api/budget-items", new {
			categoryId = 999999,
			description = "Aluguel",
			unitValue = 1000m,
			quantity = 1
		});

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Create_WithCategoryFromAnotherUser_ReturnsBadRequest() {
		var owner = await CreateAuthenticatedClientAsync();
		var intruder = await CreateAuthenticatedClientAsync();
		var category = await CreateCategoryAsync(owner);

		var response = await intruder.PostAsJsonAsync("/api/budget-items", new {
			categoryId = category.Id,
			description = "Aluguel",
			unitValue = 1000m,
			quantity = 1
		});

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Update_ExistingItem_ReturnsUpdated() {
		var client = await CreateAuthenticatedClientAsync();
		var category = await CreateCategoryAsync(client);
		var item = await CreateBudgetItemAsync(client, category.Id);

		var response = await client.PutAsJsonAsync($"/api/budget-items/{item.Id}", new {
			categoryId = category.Id,
			description = "Aluguel reajustado",
			unitValue = 1200m,
			quantity = 1
		});

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var updated = await response.Content.ReadFromJsonAsync<BudgetItemResponse>();
		Assert.Equal("Aluguel reajustado", updated!.Description);
		Assert.Equal(1200m, updated.BudgetTotal);
	}

	[Fact]
	public async Task Update_NonExistentItem_ReturnsNotFound() {
		var client = await CreateAuthenticatedClientAsync();
		var category = await CreateCategoryAsync(client);

		var response = await client.PutAsJsonAsync("/api/budget-items/999999", new {
			categoryId = category.Id,
			description = "X",
			unitValue = 1m,
			quantity = 1
		});

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task Delete_ExistingItem_ReturnsNoContent() {
		var client = await CreateAuthenticatedClientAsync();
		var category = await CreateCategoryAsync(client);
		var item = await CreateBudgetItemAsync(client, category.Id);

		var response = await client.DeleteAsync($"/api/budget-items/{item.Id}");

		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		Assert.Empty((await client.GetFromJsonAsync<List<BudgetItemResponse>>("/api/budget-items"))!);
	}

	[Fact]
	public async Task Delete_NonExistentItem_ReturnsNotFound() {
		var client = await CreateAuthenticatedClientAsync();

		var response = await client.DeleteAsync("/api/budget-items/999999");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}
}
