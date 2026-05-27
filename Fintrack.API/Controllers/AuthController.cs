using BCrypt.Net;
using Fintrack.API.Data;
using Fintrack.API.DTOs.Auth;
using Fintrack.API.Models;
using Fintrack.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase {
	private readonly AppDbContext _context;
	private readonly JwtService _jwtService;

	public AuthController(AppDbContext context, JwtService jwtService) {
		_context = context;
		_jwtService = jwtService;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register(RegisterDto dto) {
		var userExists = await _context.Users.AnyAsync(u => u.Username == dto.Username);

		if (userExists) {
			return BadRequest(new {message = "Username already exists"});
		}

		var user = new User {
			Username = dto.Username,
			Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
			TsCreated = DateTime.UtcNow
		};

		_context.Users.Add(user);

		await _context.SaveChangesAsync();

		var token = _jwtService.GenerateToken(user);

		return Ok(new AuthResponseDto{Token = token});
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login(LoginDto dto ) {
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);

		if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)) {
			return Unauthorized(new {message = "Invalid credentials"});
		}

		var token = _jwtService.GenerateToken(user);

		return Ok(new AuthResponseDto {Token = token});
	}
}
