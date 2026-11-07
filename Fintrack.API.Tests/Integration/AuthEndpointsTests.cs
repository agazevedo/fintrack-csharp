using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fintrack.API.DTOs.Auth;
using Fintrack.API.Tests.Helpers;

namespace Fintrack.API.Tests.Integration;

public class AuthEndpointsTests : IntegrationTestBase {
	public AuthEndpointsTests(FintrackApiFactory factory) : base(factory) { }

	[Fact]
	public async Task Register_ReturnsToken() {
		var client = Factory.CreateClient();

		var response = await client.PostAsJsonAsync("/api/auth/register", new { username = NewUsername(), password = DefaultPassword });

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
		Assert.False(string.IsNullOrWhiteSpace(body!.Token));

		// Garante que a API usa a configuração JWT de teste, não o appsettings local.
		Assert.Equal(TestHelpers.JwtIssuer, new JwtSecurityTokenHandler().ReadJwtToken(body.Token).Issuer);
	}

	[Fact]
	public async Task Register_DuplicateUsername_ReturnsBadRequest() {
		var client = Factory.CreateClient();
		var username = NewUsername();
		await client.PostAsJsonAsync("/api/auth/register", new { username, password = DefaultPassword });

		var response = await client.PostAsJsonAsync("/api/auth/register", new { username, password = DefaultPassword });

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Register_PasswordTooShort_ReturnsBadRequest() {
		var client = Factory.CreateClient();

		var response = await client.PostAsJsonAsync("/api/auth/register", new { username = NewUsername(), password = "123" });

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Login_ValidCredentials_ReturnsToken() {
		var client = Factory.CreateClient();
		var username = NewUsername();
		await client.PostAsJsonAsync("/api/auth/register", new { username, password = DefaultPassword });

		var response = await client.PostAsJsonAsync("/api/auth/login", new { username, password = DefaultPassword });

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
		Assert.False(string.IsNullOrWhiteSpace(body!.Token));
	}

	[Fact]
	public async Task Login_WrongPassword_ReturnsUnauthorized() {
		var client = Factory.CreateClient();
		var username = NewUsername();
		await client.PostAsJsonAsync("/api/auth/register", new { username, password = DefaultPassword });

		var response = await client.PostAsJsonAsync("/api/auth/login", new { username, password = "senha-errada" });

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized() {
		var response = await Factory.CreateClient().GetAsync("/api/test");

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task ProtectedEndpoint_WithInvalidToken_ReturnsUnauthorized() {
		var client = Factory.CreateClient();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "token-invalido");

		var response = await client.GetAsync("/api/test");

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task ProtectedEndpoint_WithValidToken_ReturnsOk() {
		var client = await CreateAuthenticatedClientAsync();

		var response = await client.GetAsync("/api/test");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}
}
