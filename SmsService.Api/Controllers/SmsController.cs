using Microsoft.AspNetCore.Mvc;

namespace SmsService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsController : ControllerBase
    {
        [HttpGet("notify-by-sms", Name = "SendSms")]
        public ActionResult<Guid> SendSms(Guid reviewId, string from, string phoneNumber)
        {
            // do this, do that
            return Ok(Guid.NewGuid());
        }

        [HttpGet]
        public IActionResult HealthCheck()
        {
            return Ok();
        }
    }
}

//"http://smsservice-api:8080/api/Sms/notify-by-sms?id=300FB1A4-8EB4-412D-9CF3-B5F2D77A3825&from=from&to=to"