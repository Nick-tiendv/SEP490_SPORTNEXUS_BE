using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.API.Controllers
{
    [Route("api/v1/tournaments")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _service;
        public TournamentController(ITournamentService service) { _service = service; }


        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> CreateTournament([FromBody] CreateTournamentRequest request)
        {
            var res = await _service.CreateTournamentAsync(request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet]
        public async Task<IActionResult> GetTournaments()
        {
            var res = await _service.GetTournamentsAsync();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{id}/bracket")]
        public async Task<IActionResult> GetBracket(Guid id)
        {
            var res = await _service.GetBracketAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("{id}/join")]
        [Authorize]
        public async Task<IActionResult> JoinTournament(Guid id, [FromQuery] Guid? teamId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var res = await _service.RegisterTournamentAsync(userId, id, teamId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("{id}/execute-random-pairing")]
        [Authorize]
        public async Task<IActionResult> ExecutePairing(Guid id)
        {
            var res = await _service.ExecuteRandomPairingAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("{id}/generate-bracket")]
        [Authorize]
        public async Task<IActionResult> GenerateBracket(Guid id)
        {
            var res = await _service.GenerateBracketAsync(id);
            return StatusCode(res.StatusCode, res);
        }
    }

    [Route("api/v1/matches")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly ITournamentService _service;
        public MatchController(ITournamentService service) { _service = service; }

        [HttpPut("{matchId}/score")]
        [Authorize]
        public async Task<IActionResult> UpdateScore(Guid matchId, [FromQuery] int scoreP1, [FromQuery] int scoreP2, [FromQuery] Guid winnerId)
        {
            var res = await _service.UpdateScoreAndAdvanceAsync(matchId, scoreP1, scoreP2, winnerId);
            return StatusCode(res.StatusCode, res);
        }
    }
}