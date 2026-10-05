using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.API.Controllers
{
    [Route("api/v1/courts")]
    [ApiController]
    public class CourtSlotController : ControllerBase
    {
        private readonly ICourtSlotService _service;
        public CourtSlotController(ICourtSlotService service)
        {
            _service = service;
        }

        [HttpGet("{courtId}/slots")]
        public async Task<IActionResult> GetSlots(Guid courtId, [FromQuery] DateTime date)
        {
            var res = await _service.GetTimeGridAsync(courtId, date);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("{courtId}/slots/generate")]
        [Authorize]
        public async Task<IActionResult> GenerateSlots(Guid courtId, [FromBody] GenerateCourtSlotsRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid ownerId)) return Unauthorized();

            var res = await _service.GenerateCourtSlotsAsync(courtId, ownerId, request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("/api/v1/slots/{slotId}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateSlotStatus(Guid slotId, [FromBody] UpdateSlotStatusRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid ownerId)) return Unauthorized();

            var res = await _service.UpdateSlotStatusAsync(slotId, ownerId, request);
            return StatusCode(res.StatusCode, res);
        }
    }
}
