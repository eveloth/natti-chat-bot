using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using NattiChatBot.Contracts.Requests;
using NattiChatBot.Domain;
using NattiChatBot.Domain.Enums;
using NattiChatBot.Filters;
using NattiChatBot.Services.Interfaces;

namespace NattiChatBot.Controllers;

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
    [ProducesResponseType(typeof(IEnumerable<Token>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var tokens = await _tokenService.GetAll(ct);
        return Ok(tokens);
    }

    [HttpPost]
    [ValitatePfzToken(AccessType.Admin)]
    [ProducesResponseType(typeof(Token), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] TokenRequest request, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(request, ct);

        var token = new Token();
        _mapper.Map(request, token);
        var createdToken = await _tokenService.Add(token, ct);

        return CreatedAtAction(nameof(Add), createdToken);
    }

    [HttpPut]
    [Route("{id:long}")]
    [ValitatePfzToken(AccessType.Admin)]
    [ProducesResponseType(typeof(Token), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken ct)
    {
        await _tokenService.Delete(id, ct);
        return NoContent();
    }

    [HttpGet]
    [Route("info")]
    [ValitatePfzToken(AccessType.User)]
    [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTokenInfo(CancellationToken ct)
    {
        var requestInitiatorToken = HttpContext.GetPfzToken();
        var existingToken = await _tokenService.Get(requestInitiatorToken, ct);
        return Ok(existingToken);
    }
}