using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.API.Controllers
{
    [Route("api/v1/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;
        public PaymentController(IPaymentService service) { _service = service; }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromHeader(Name = "X-Signature")] string signature, [FromBody] object payload)
        {
            string raw = System.Text.Json.JsonSerializer.Serialize(payload);
            var res = await _service.ProcessWebhookAsync("VNPay", signature, raw);
            return StatusCode(res.StatusCode, res);
        }
    }

    [Route("api/v1/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;
        public NotificationController(INotificationService service) { _service = service; }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            var res = await _service.GetUserNotificationsAsync(userId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("{id}/read")]
        [Authorize]
        public async Task<IActionResult> MarkRead(Guid id)
        {
            var res = await _service.MarkAsReadAsync(id);
            return StatusCode(res.StatusCode, res);
        }
    }
}