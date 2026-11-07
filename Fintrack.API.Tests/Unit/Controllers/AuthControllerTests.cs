using Fintrack.API.Controllers;
using Fintrack.API.Data;
using Fintrack.API.DTOs.Auth;
using Fintrack.API.Services;
using Fintrack.API.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.API.Tests.Unit.Controllers;

public class AuthControllerTests {
	private readonly AppDbContext _context = TestHelpers.CreateInMemoryContext();
	private readonly AuthController _controller;

	public AuthControllerTests() {
		_controller = new AuthController(_context, new JwtService(TestHelpers.CreateJwtConfiguration()));
	}

	[Fact]
	public async Task Register_NewUser_ReturnsTokenAndStoresHashedPassword() {
		var result = await _controller.Register(new RegisterDto { Username = "alice", Password = "secret123" });

		var ok = Assert.IsType<OkObjectResult>(result);
		Assert.False(string.IsNullOrWhiteSpace(Assert.IsType<AuthResponseDto>(ok.Value).Token));

		var user = await _context.Users.SingleAsync();
		Assert.NotEqual("secret123", user.Password);
		Assert.True(BCrypt.Net.BCrypt.Verify("secret123", user.Password));
	}

	[Fact]
	public async Task Register_ExistingUsername_ReturnsBadRequest() {
		await _controller.Register(new RegisterDto { Username = "alice", Password = "secret123" });

		var result = await _controller.Register(new RegisterDto { Username = "alice", Password = "outra-senha" });

		Assert.IsType<BadRequestObjectResult>(result);
		Assert.Equal(1, await _context.Users.CountAsync());
	}

	[Fact]
	public async Task Login_ValidCredentials_ReturnsToken() {
		await _controller.Register(new RegisterDto { Username = "alice", Password = "secret123" });

		var result = await _controller.Login(new LoginDto { Username = "alice", Password = "secret123" });

		var ok = Assert.IsType<OkObjectResult>(result);
		Assert.False(string.IsNullOrWhiteSpace(Assert.IsType<AuthResponseDto>(ok.Value).Token));
	}

	[Theory]
	[InlineData("alice", "senha-errada")]
	[InlineData("inexistente", "secret123")]
	public async Task Login_InvalidCredentials_ReturnsUnauthorized(string username, string password) {
		await _controller.Register(new RegisterDto { Username = "alice", Password = "secret123" });

		var result = await _controller.Login(new LoginDto { Username = username, Password = password });

		Assert.IsType<UnauthorizedObjectResult>(result);
	}
}
