using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using NattiChatBot.Contracts.Queries;
using NattiChatBot.Data.Filters;
using NattiChatBot.Domain;
using NattiChatBot.Domain.Enums;
using NattiChatBot.Filters;
using NattiChatBot.Services.Interfaces;

namespace NattiChatBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _statsService;
        private readonly IMapper _mapper;

        public StatsController(IStatsService statsService, IMapper mapper)
        {
            _statsService = statsService;
            _mapper = mapper;
        }

        [HttpGet]
        [ValitatePfzToken(AccessType.User)]
        [ProducesResponseType(typeof(List<Stats>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] StatsFilterQuery filterQuery,
            CancellationToken ct
        )
        {
            var filter = _mapper.Map<DateFilter>(filterQuery);

            return Ok(await _statsService.GetAll(filter, ct));
        }

        [HttpGet]
        [Route("{date:datetime}")]
        [ValitatePfzToken(AccessType.User)]
        [ProducesResponseType(typeof(Stats), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromRoute] DateTime date, CancellationToken ct)
        {
            var dateOnly = DateOnly.FromDateTime(date);

            return Ok(await _statsService.Get(dateOnly, ct));
        }
    }
}