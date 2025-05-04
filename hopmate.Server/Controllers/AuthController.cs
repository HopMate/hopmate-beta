using Microsoft.AspNetCore.Mvc;
using hopmate.Server.Models.Dto;
using hopmate.Server.Services;

namespace hopmate.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var (success, errors) = await _authService.RegisterAsync(registerDto);

            if (success)
            {
                return Ok(new { Message = "User and passenger created successfully." });
            }

            return BadRequest(new { Errors = errors });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var (token, message) = await _authService.LoginAsync(loginDto);

            if (!string.IsNullOrEmpty(token))
            {
                return Ok(new { Token = token });
            }

            return Unauthorized(new { Message = message });
        }
    }
}