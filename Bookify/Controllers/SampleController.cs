using Bookify.Models.Services;
using Bookify.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmsServiceNamespace;

namespace Bookify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController(
        IDefaultEmailClient defaultMailClient,
        IOptions<SmsClientOptions> smsOptions) : ControllerBase
    {
        //var requestString = "http://mailservice-api:8080/api/Main/send?id=%7B6FB4DE5B-26CC-4571-9A21-A4642660985C%7D&from=from&to=to&subject=subject&body=body";
        //public MailClientOptions MailClientOptions => options.Value;
        public SmsClientOptions SmsClientOptions => smsOptions.Value;

        [HttpGet("receive-email-confirmation")]
        public async Task<ActionResult<string>> ReceiveConfirmation()
        {
            //MailClient mailClient = new MailClient(MailClientOptions.BaseUrl, new HttpClient());
            //var response = await mailClient.ForwardEmailAsync(Guid.NewGuid(), "from", "to", "subject", "body");

            var response = await defaultMailClient.ForwardEmailAsync(Guid.NewGuid(), "from", "to", "subject", "body");
            if (response.IsSuccess)
            {
                var result = $"We have sent you a confirmation email. Email ID: {response.Value}";
                return Ok(result);
            }
            else
            {
                return BadRequest("Email confirmation failed");
            }
        }

        [HttpGet("receive-sms-notification")]
        public async Task<ActionResult<string>> SendNotificationSms()
        {
            //[2]
            SmsClient smsClient = new SmsClient(SmsClientOptions.BaseUrl, new HttpClient());
            var smsId = await smsClient.SendSmsAsync(Guid.NewGuid(), "Bookify", "+799912345678");

            var result = $"Your review was published (notification ID: {smsId})";
            return Ok(result);
        }
    }
}