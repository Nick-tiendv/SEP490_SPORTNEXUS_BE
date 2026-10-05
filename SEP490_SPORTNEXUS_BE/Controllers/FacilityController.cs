using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.API.Controllers
{
    [Route("api/v1/facilities")]
    [ApiController]
    public class FacilityController : ControllerBase
    {
        private readonly IFacilityService _service;
        public FacilityController(IFacilityService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] double? lat, [FromQuery] double? lng, [FromQuery] double? radius, [FromQuery] Guid? categoryId, [FromQuery] Guid? amenityId, [FromQuery] string? status)
        {
            var res = await _service.SearchFacilitiesAsync(lat, lng, radius, categoryId, amenityId, status);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(Guid id)
        {
            var res = await _service.GetFacilityDetailsAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost]
        [Authorize] // Assuming they have token decoding
        public async Task<IActionResult> Create([FromBody] CreateFacilityRequest request)
        {
            // Note: Replace with actual claim extraction logic
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid ownerId)) return Unauthorized();

            var res = await _service.CreateFacilityAsync(ownerId, request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFacilityRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid ownerId)) return Unauthorized();

            var res = await _service.UpdateFacilityAsync(id, ownerId, request);
            return StatusCode(res.StatusCode, res);
        }
    }
}
