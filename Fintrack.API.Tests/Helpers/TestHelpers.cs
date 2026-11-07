using System.Security.Claims;
using Fintrack.API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Fintrack.API.Tests.Helpers;

public static class TestHelpers {
	public const string JwtKey = "fintrack-test-key-com-pelo-menos-32-bytes-de-tamanho";
	public const string JwtIssuer = "fintrack-api-tests";
	public const string JwtAudience = "fintrack-client-tests";

	public static Dictionary<string, string?> JwtSettings => new() {
		["Jwt:Key"] = JwtKey,
		["Jwt:Issuer"] = JwtIssuer,
		["Jwt:Audience"] = JwtAudience
	};

	public static IConfiguration CreateJwtConfiguration() =>
		new ConfigurationBuilder().AddInMemoryCollection(JwtSettings).Build();

	// Cada chamada cria um banco isolado, evitando interferência entre testes.
	public static AppDbContext CreateInMemoryContext() =>
		new(new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options);

	// Simula um usuário autenticado no controller (claim lida via ClaimTypes.NameIdentifier).
	public static T WithUser<T>(this T controller, int userId) where T : ControllerBase {
		var identity = new ClaimsIdentity(
			new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) },
			"Test"
		);

		controller.ControllerContext = new ControllerContext {
			HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
		};

		return controller;
	}
}
