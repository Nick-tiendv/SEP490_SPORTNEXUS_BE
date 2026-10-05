using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.API.Controllers
{
    [Route("api/v1/ai/sessions")]
    [ApiController]
    public class AiAssistantController : ControllerBase
    {
        private readonly IAiAssistantService _service;
        public AiAssistantController(IAiAssistantService service) { _service = service; }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateSession()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var res = await _service.CreateSessionAsync(userId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetSessions()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var res = await _service.GetSessionHistoryAsync(userId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{id}/messages")]
        [Authorize]
        public async Task<IActionResult> GetMessages(Guid id)
        {
            var res = await _service.GetMessagesAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("{id}/chat")]
        [Authorize]
        public async Task<IActionResult> Chat(Guid id, [FromBody] string message)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var res = await _service.ProcessChatAsync(userId, id, message);
            return StatusCode(res.StatusCode, res);
        }
    }
}