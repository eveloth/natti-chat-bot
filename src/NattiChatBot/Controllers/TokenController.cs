using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using NattiChatBot.Contracts.Requests;
using NattiChatBot.Domain;
using NattiChatBot.Domain.Enums;
using NattiChatBot.Filters;
using NattiChatBot.Services.Interfaces;

namespace NattiChatBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IValidator<TokenRequest> _validator;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public TokenController(
            IValidator<TokenRequest> validator,
            IMapper mapper,
            ITokenService tokenService
        )
        {
            _validator = validator;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpGet]
        [ValitatePfzToken(AccessType.Admin)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var tokens = await _tokenService.GetAll(ct);
            return Ok(tokens);
        }

        [HttpPost]
        [ValitatePfzToken(AccessType.Admin)]
        public async Task<IActionResult> Add([FromBody] TokenRequest request, CancellationToken ct)
        {
            await _validator.ValidateAndThrowAsync(request, ct);

            var token = _mapper.Map<Token>(request);

            var createdToken = await _tokenService.Add(token, ct);
            return CreatedAtAction(nameof(Add), createdToken);
        }

        [HttpPut]
        [Route("{id:long}")]
        [ValitatePfzToken(AccessType.Admin)]
        public async Task<IActionResult> Update(
            [FromRoute] long id,
            [FromBody] TokenRequest request,
            CancellationToken ct
        )
        {
            await _validator.ValidateAndThrowAsync(request, ct);

            var token = _mapper.Map<Token>(request);
            token.Id = id;

            var updatedToken = await _tokenService.Update(token, ct);
            return CreatedAtAction(nameof(Add), updatedToken);
        }

        [HttpDelete]
        [Route("{id:long}")]
        [ValitatePfzToken(AccessType.Admin)]
        public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken ct)
        {
            await _tokenService.Delete(id, ct);
            return NoContent();
        }
    }
}