using Microsoft.AspNetCore.Mvc;

namespace MailService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        [HttpGet("forwardEmail", Name = "ForwardEmail")]
        public ActionResult<Guid> ForwardEmail(Guid id, string from, string to, string subject, string body)
        {
            return Ok(Guid.NewGuid());
        }

        [HttpGet]
        public IActionResult HealthCheck()
        {
            return Ok();
        }
    }

}  //"http://mailservice-api:8080/api/Mail/forwardEmail?id=CD57CF32-872F-41F1-A2A5-0CCCE26E6224&from=from&to=to&subject=subject&body=body"