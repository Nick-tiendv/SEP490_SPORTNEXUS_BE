using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.API.Controllers
{
    [Route("api/v1/checkin")]
    [ApiController]
    public class CheckInController : ControllerBase
    {
        private readonly ICheckInService _service;
        public CheckInController(ICheckInService service) { _service = service; }

        [HttpPost("scan")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> ScanQr([FromQuery] Guid facilityId, [FromQuery] string qrCode)
        {
            var res = await _service.ScanQrCodeAsync(facilityId, qrCode);
            return StatusCode(res.StatusCode, res);
        }
    }

    [Route("api/v1/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ICheckInService _service;
        public ReviewController(ICheckInService service) { _service = service; }

        [HttpPost("fair-play")]
        [Authorize]
        public async Task<IActionResult> RateFairPlay([FromBody] RateFairPlayDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var res = await _service.RateFairPlayAsync(userId, dto.RevieweeId, dto.ContextId, dto.Score, dto.Comment);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("facility")]
        [Authorize]
        public async Task<IActionResult> RateFacility([FromBody] RateFacilityDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var res = await _service.AddFacilityReviewAsync(userId, dto.FacilityId, dto.Rating, dto.Comment);
            return StatusCode(res.StatusCode, res);
        }
    }

    public class RateFairPlayDto { public Guid RevieweeId { get; set; } public Guid ContextId { get; set; } public int Score { get; set; } public string Comment { get; set; } = null!; }
    public class RateFacilityDto { public Guid FacilityId { get; set; } public int Rating { get; set; } public string Comment { get; set; } = null!; }
}