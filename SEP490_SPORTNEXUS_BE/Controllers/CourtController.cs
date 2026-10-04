using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.API.Controllers
{
    [Route("api/v1/facilities/{facilityId}/courts")]
    [ApiController]
    public class CourtController : ControllerBase
    {
        private readonly ICourtService _service;
        public CourtController(ICourtService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourts(Guid facilityId)
        {
            var res = await _service.GetCourtsByFacilityAsync(facilityId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Guid facilityId, [FromBody] CreateCourtRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid ownerId)) return Unauthorized();

            var res = await _service.CreateCourtAsync(facilityId, ownerId, request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("{courtId}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid facilityId, Guid courtId, [FromBody] UpdateCourtRequest request)
        {
            // FacilityId can be validated if needed, but omitted for simplicity
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid ownerId)) return Unauthorized();

            var res = await _service.UpdateCourtAsync(courtId, ownerId, request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpDelete("{courtId}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid facilityId, Guid courtId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid ownerId)) return Unauthorized();

            var res = await _service.DeleteCourtAsync(courtId, ownerId);
            return StatusCode(res.StatusCode, res);
        }
    }
}
