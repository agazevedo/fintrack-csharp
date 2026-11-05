using System.Net;
using System.Net.Http.Json;

namespace Fintrack.API.Tests.Integration;

public class ExpensesEndpointsTests : IntegrationTestBase {
	public ExpensesEndpointsTests(FintrackApiFactory factory) : base(factory) { }

	private static object ExpensePayload(int budgetItemId, string date = "2026-11-01") => new {
		budgetItemId,
		unitValue = 150m,
		quantity = 2,
		date
	};

	private async Task<(HttpClient Client, BudgetItemResponse Item)> ArrangeBudgetItemAsync() {
		var client = await CreateAuthenticatedClientAsync();
		var category = await CreateCategoryAsync(client);
		var item = await CreateBudgetItemAsync(client, category.Id);
		return (client, item);
	}

	[Fact]
	public async Task GetAll_WithoutToken_ReturnsUnauthorized() {
		var response = await Factory.CreateClient().GetAsync("/api/expenses");

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task Create_ThenGetAll_ReturnsExpense() {
		var (client, item) = await ArrangeBudgetItemAsync();

		var response = await client.PostAsJsonAsync("/api/expenses", ExpensePayload(item.Id));
		Assert.Equal(HttpStatusCode.Created, response.StatusCode);

		var expenses = await client.GetFromJsonAsync<List<ExpenseResponse>>("/api/expenses");
		var expense = Assert.Single(expenses!);
		Assert.Equal(item.Id, expense.BudgetItemId);
		Assert.Equal(300m, expense.Total);
		Assert.Equal(new DateOnly(2026, 11, 1), expense.Date);
	}

	[Fact]
	public async Task Create_UpdatesBudgetItemSpentAndRemaining() {
		var (client, item) = await ArrangeBudgetItemAsync();

		await client.PostAsJsonAsync("/api/expenses", ExpensePayload(item.Id));

		var items = await client.GetFromJsonAsync<List<BudgetItemResponse>>("/api/budget-items");
		var updated = Assert.Single(items!);
		Assert.Equal(300m, updated.Spent);
		Assert.Equal(item.BudgetTotal - 300m, updated.Remaining);
	}

	[Fact]
	public async Task Create_WithInvalidBudgetItem_ReturnsBadRequest() {
		var client = await CreateAuthenticatedClientAsync();

		var response = await client.PostAsJsonAsync("/api/expenses", ExpensePayload(999999));

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Create_WithBudgetItemFromAnotherUser_ReturnsBadRequest() {
		var (_, item) = await ArrangeBudgetItemAsync();
		var intruder = await CreateAuthenticatedClientAsync();

		var response = await intruder.PostAsJsonAsync("/api/expenses", ExpensePayload(item.Id));

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Update_ExistingExpense_ReturnsUpdated() {
		var (client, item) = await ArrangeBudgetItemAsync();
		var created = await (await client.PostAsJsonAsync("/api/expenses", ExpensePayload(item.Id)))
			.Content.ReadFromJsonAsync<ExpenseResponse>();

		var response = await client.PutAsJsonAsync($"/api/expenses/{created!.Id}", new {
			budgetItemId = item.Id,
			unitValue = 50m,
			quantity = 1,
			date = "2026-11-02"
		});

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var updated = await response.Content.ReadFromJsonAsync<ExpenseResponse>();
		Assert.Equal(50m, updated!.Total);
		Assert.Equal(new DateOnly(2026, 11, 2), updated.Date);
	}

	[Fact]
	public async Task Update_NonExistentExpense_ReturnsNotFound() {
		var (client, item) = await ArrangeBudgetItemAsync();

		var response = await client.PutAsJsonAsync("/api/expenses/999999", ExpensePayload(item.Id));

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task Delete_ExistingExpense_ReturnsNoContent() {
		var (client, item) = await ArrangeBudgetItemAsync();
		var created = await (await client.PostAsJsonAsync("/api/expenses", ExpensePayload(item.Id)))
			.Content.ReadFromJsonAsync<ExpenseResponse>();

		var response = await client.DeleteAsync($"/api/expenses/{created!.Id}");

		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		Assert.Empty((await client.GetFromJsonAsync<List<ExpenseResponse>>("/api/expenses"))!);
	}

	[Fact]
	public async Task Delete_ExpenseFromAnotherUser_ReturnsNotFound() {
		var (client, item) = await ArrangeBudgetItemAsync();
		var created = await (await client.PostAsJsonAsync("/api/expenses", ExpensePayload(item.Id)))
			.Content.ReadFromJsonAsync<ExpenseResponse>();
		var intruder = await CreateAuthenticatedClientAsync();

		var response = await intruder.DeleteAsync($"/api/expenses/{created!.Id}");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}
}
