using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fintrack.API.Models;
using Fintrack.API.Services;
using Fintrack.API.Tests.Helpers;
using Microsoft.IdentityModel.Tokens;

namespace Fintrack.API.Tests.Unit.Services;

public class JwtServiceTests {
	private readonly JwtService _service = new(TestHelpers.CreateJwtConfiguration());
	private readonly User _user = new() { Id = 42, Username = "alice" };

	[Fact]
	public void GenerateToken_IncludesUserClaims() {
		var token = new JwtSecurityTokenHandler().ReadJwtToken(_service.GenerateToken(_user));

		Assert.Equal("42", token.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);
		Assert.Equal("alice", token.Claims.Single(c => c.Type == ClaimTypes.Name).Value);
	}

	[Fact]
	public void GenerateToken_UsesConfiguredIssuerAudienceAndSevenDayExpiry() {
		var token = new JwtSecurityTokenHandler().ReadJwtToken(_service.GenerateToken(_user));

		Assert.Equal(TestHelpers.JwtIssuer, token.Issuer);
		Assert.Contains(TestHelpers.JwtAudience, token.Audiences);
		Assert.InRange(token.ValidTo, DateTime.UtcNow.AddDays(7).AddMinutes(-1), DateTime.UtcNow.AddDays(7).AddMinutes(1));
	}

	[Fact]
	public void GenerateToken_IsValidWithConfiguredKey() {
		var parameters = new TokenValidationParameters {
			ValidIssuer = TestHelpers.JwtIssuer,
			ValidAudience = TestHelpers.JwtAudience,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestHelpers.JwtKey))
		};

		var principal = new JwtSecurityTokenHandler()
			.ValidateToken(_service.GenerateToken(_user), parameters, out _);

		Assert.Equal("42", principal.FindFirstValue(ClaimTypes.NameIdentifier));
	}
}
