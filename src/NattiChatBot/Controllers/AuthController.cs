using Microsoft.AspNetCore.Mvc;
using NattiChatBot.Services.Interfaces;

namespace NattiChatBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ITokenService tokenService, ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(string token, CancellationToken ct)
        {
            var existingToken = await _tokenService.Get(token, ct);

            return existingToken is null ? Ok() : Unauthorized();
        }
    }
}