using Microsoft.AspNetCore.Mvc;
using NattiChatBot.Contracts.Requests;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var existingToken = await _tokenService.Get(request.AccessToken, ct);

            return existingToken is null ? Unauthorized() : Ok();
        }
    }
}