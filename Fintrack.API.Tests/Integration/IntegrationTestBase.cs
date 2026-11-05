using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fintrack.API.DTOs.Auth;

namespace Fintrack.API.Tests.Integration;

/// <summary>
/// Base para testes de integração. A factory (e o banco) é compartilhada pelos
/// testes da mesma classe; o isolamento vem de cada teste usar um usuário novo.
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<FintrackApiFactory> {
	protected const string DefaultPassword = "secret123";

	protected readonly FintrackApiFactory Factory;

	protected IntegrationTestBase(FintrackApiFactory factory) {
		Factory = factory;
	}

	protected static string NewUsername() => $"user_{Guid.NewGuid():N}";

	protected async Task<HttpClient> CreateAuthenticatedClientAsync() {
		var client = Factory.CreateClient();

		var response = await client.PostAsJsonAsync("/api/auth/register", new {
			username = NewUsername(),
			password = DefaultPassword
		});
		response.EnsureSuccessStatusCode();

		var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);

		return client;
	}

	protected static async Task<CategoryResponse> CreateCategoryAsync(HttpClient client, string name = "Casa") {
		var response = await client.PostAsJsonAsync("/api/categories", new { name, type = "expense" });
		response.EnsureSuccessStatusCode();
		return (await response.Content.ReadFromJsonAsync<CategoryResponse>())!;
	}

	protected static async Task<BudgetItemResponse> CreateBudgetItemAsync(HttpClient client, int categoryId, string description = "Aluguel") {
		var response = await client.PostAsJsonAsync("/api/budget-items", new {
			categoryId,
			description,
			unitValue = 1000m,
			quantity = 2
		});
		response.EnsureSuccessStatusCode();
		return (await response.Content.ReadFromJsonAsync<BudgetItemResponse>())!;
	}

	protected record CategoryResponse(int Id, int UserId, string Name, string Type);

	protected record BudgetItemResponse(
		int Id,
		int CategoryId,
		string Description,
		decimal UnitValue,
		int Quantity,
		decimal BudgetTotal,
		decimal Spent,
		decimal Remaining,
		CategoryResponse? Category
	);

	protected record ExpenseResponse(int Id, int BudgetItemId, decimal UnitValue, int Quantity, DateOnly Date, decimal Total);
}
