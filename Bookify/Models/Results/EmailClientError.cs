using Bookify.Domain;

namespace Bookify.Models.Results
{
    public class EmailClientError
    {
        public static Error ForwardMessage = new(
            "MailClient.ForwardMessage",
            "Error occurred when forwarding the confirmation email");
    }
}
