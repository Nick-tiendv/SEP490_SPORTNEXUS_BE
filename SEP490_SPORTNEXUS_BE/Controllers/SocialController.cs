using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.API.Controllers
{
    [Route("api/v1/lfgs")]
    [ApiController]
    public class LfgController : ControllerBase
    {
        private readonly ILfgService _service;
        public LfgController(ILfgService service) { _service = service; }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateLfgCard(Guid courtSlotId, int slotsNeeded, string? skillTag, decimal totalAmount)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var res = await _service.CreateLfgCardAsync(userId, courtSlotId, slotsNeeded, skillTag, totalAmount);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("{cardId}/claim")]
        [Authorize]
        public async Task<IActionResult> ClaimSlot(Guid cardId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var res = await _service.ClaimSlotAsync(userId, cardId);
            return StatusCode(res.StatusCode, res);
        }
    }

    [Route("api/v1/community")]
    [ApiController]
    public class CommunityController : ControllerBase
    {
        private readonly ICommunityService _service;
        public CommunityController(ICommunityService service) { _service = service; }

        [HttpPost("posts")]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var res = await _service.CreatePostAsync(userId, dto.Content, dto.MediaUrls);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("posts/{postId}/comments")]
        [Authorize]
        public async Task<IActionResult> Comment(Guid postId, [FromBody] string content)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var res = await _service.CommentOnPostAsync(userId, postId, content);
            return StatusCode(res.StatusCode, res);
        }
    }

    public class CreatePostDto {
        public string Content { get; set; } = null!;
        public List<string> MediaUrls { get; set; } = new List<string>();
    }
}