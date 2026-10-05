using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.API.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;
        public BookingController(IBookingService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var res = await _service.CreateBookingAsync(userId, request);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("{bookingId}/join")]
        [Authorize]
        public async Task<IActionResult> JoinBooking(Guid bookingId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var res = await _service.JoinBookingAsync(userId, bookingId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{bookingId}/qr-ticket")]
        [Authorize]
        public async Task<IActionResult> GetQrTicket(Guid bookingId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var res = await _service.GetDynamicQrTicketAsync(userId, bookingId);
            return StatusCode(res.StatusCode, res);
        }
    }
}