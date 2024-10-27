using Bookify.Domain;

namespace Bookify.Models.Results
{
    public class SmsClientError
    {
        public static Error SendSMS = new(
            "SmsClient.SendSMS",
            "Error occurred when the notification SMS");
    }
}
