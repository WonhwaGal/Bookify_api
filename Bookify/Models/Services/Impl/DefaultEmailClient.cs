using Bookify.Domain;
using Bookify.Models.Results;
using Bookify.Options;
using EmailServiceNamespace;
using Microsoft.Extensions.Options;

namespace Bookify.Models.Services.Impl
{
    public class DefaultEmailClient : IDefaultEmailClient
    {
        private EmailClient _mailClient;
        private ILogger<DefaultEmailClient> _logger;

        public EmailClientOptions MailClientOptions { get; }

        public DefaultEmailClient(
            IOptions<EmailClientOptions> options,
            ILogger<DefaultEmailClient> logger,
            HttpClient httpClient)
        {
            MailClientOptions = options.Value;
            _logger = logger;
            _mailClient = new EmailClient(MailClientOptions.BaseUrl, httpClient);
        }

        public async Task<Result<Guid>> ForwardEmailAsync(Guid id, string from, string to, string subject, string body)
        {
            try
            {
                var response = await _mailClient.ForwardEmailAsync(Guid.NewGuid(), "from", "to", "subject", "body");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Forward Email Error");
                return Result.Failure<Guid>(EmailClientError.ForwardMessage);
            }
        }
    }
}
