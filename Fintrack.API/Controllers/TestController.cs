using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fintrack.API.Controllers;

[ApiController]
[Route("api/test")]
[Authorize]
public class TestController : ControllerBase {
	[HttpGet]
	public IActionResult Get() {
		return Ok(new {message = "Authorized"});
	}
}
