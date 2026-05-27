using Fintrack.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fintrack.API.Services;

public class JwtService {
	private readonly IConfiguration _configuration;

	public JwtService(IConfiguration configuration) {
		_configuration = configuration;
	}

	public string GenerateToken(User user) {
		var key = Encoding.UTF8.GetBytes(
			_configuration["Jwt:Key"]!
		);

		var claims = new[] {
			new Claim(
				ClaimTypes.NameIdentifier,
				user.Id.ToString()
			),

			new Claim(
				ClaimTypes.Name,
				user.Username
			)
		};

		var credentials = new SigningCredentials(
			new SymmetricSecurityKey(key),
			SecurityAlgorithms.HmacSha256
		);

		var token = new JwtSecurityToken(
			issuer: _configuration["Jwt:Issuer"],
			audience: _configuration["Jwt:Audience"],
			claims: claims,
			expires: DateTime.UtcNow.AddDays(7),
			signingCredentials: credentials
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}
