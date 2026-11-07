using System.Net;
using System.Net.Http.Json;

namespace Fintrack.API.Tests.Integration;

public class CategoriesEndpointsTests : IntegrationTestBase {
	public CategoriesEndpointsTests(FintrackApiFactory factory) : base(factory) { }

	[Fact]
	public async Task GetAll_WithoutToken_ReturnsUnauthorized() {
		var response = await Factory.CreateClient().GetAsync("/api/categories");

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task Create_ThenGetAll_ReturnsCreatedCategory() {
		var client = await CreateAuthenticatedClientAsync();

		var createResponse = await client.PostAsJsonAsync("/api/categories", new { name = "Mercado", type = "expense" });
		Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

		var categories = await client.GetFromJsonAsync<List<CategoryResponse>>("/api/categories");
		var category = Assert.Single(categories!);
		Assert.Equal("Mercado", category.Name);
		Assert.Equal("expense", category.Type);
	}

	[Fact]
	public async Task Create_DuplicateName_ReturnsBadRequest() {
		var client = await CreateAuthenticatedClientAsync();
		await CreateCategoryAsync(client, "Mercado");

		var response = await client.PostAsJsonAsync("/api/categories", new { name = "mercado", type = "expense" });

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Create_MissingName_ReturnsBadRequest() {
		var client = await CreateAuthenticatedClientAsync();

		var response = await client.PostAsJsonAsync("/api/categories", new { type = "expense" });

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Update_ExistingCategory_ReturnsUpdated() {
		var client = await CreateAuthenticatedClientAsync();
		var category = await CreateCategoryAsync(client, "Mercado");

		var response = await client.PutAsJsonAsync($"/api/categories/{category.Id}", new { name = "Supermercado", type = "expense" });

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var updated = await response.Content.ReadFromJsonAsync<CategoryResponse>();
		Assert.Equal("Supermercado", updated!.Name);
	}

	[Fact]
	public async Task Update_NonExistentCategory_ReturnsNotFound() {
		var client = await CreateAuthenticatedClientAsync();

		var response = await client.PutAsJsonAsync("/api/categories/999999", new { name = "X", type = "expense" });

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task Delete_ExistingCategory_ReturnsNoContentAndRemovesIt() {
		var client = await CreateAuthenticatedClientAsync();
		var category = await CreateCategoryAsync(client);

		var response = await client.DeleteAsync($"/api/categories/{category.Id}");

		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		Assert.Empty((await client.GetFromJsonAsync<List<CategoryResponse>>("/api/categories"))!);
	}

	[Fact]
	public async Task OtherUser_CannotSeeUpdateOrDeleteCategory() {
		var owner = await CreateAuthenticatedClientAsync();
		var intruder = await CreateAuthenticatedClientAsync();
		var category = await CreateCategoryAsync(owner);

		Assert.Empty((await intruder.GetFromJsonAsync<List<CategoryResponse>>("/api/categories"))!);
		Assert.Equal(HttpStatusCode.NotFound,
			(await intruder.PutAsJsonAsync($"/api/categories/{category.Id}", new { name = "Hack", type = "expense" })).StatusCode);
		Assert.Equal(HttpStatusCode.NotFound, (await intruder.DeleteAsync($"/api/categories/{category.Id}")).StatusCode);
	}
}
