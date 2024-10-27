using Bookify.Domain;
using Bookify.Models.Results;
using Bookify.Options;
using Microsoft.Extensions.Options;
using SmsServiceNamespace;

namespace Bookify.Models.Services.Impl
{
    public class DefaultSmsClient : IDefaultSmsClient
    {
        private SmsClient _smsClient;
        private ILogger<DefaultSmsClient> _logger;

        public SmsClientOptions SmsClientOptions { get; }

        public DefaultSmsClient(
            IOptions<SmsClientOptions> options,
            ILogger<DefaultSmsClient> logger,
            HttpClient httpClient)
        {
            SmsClientOptions = options.Value;
            _smsClient = new SmsClient(SmsClientOptions.BaseUrl, httpClient);
            _logger = logger;
        }

        public async Task<Result<Guid>> SendSmsAsync(Guid reviewId, string sendingService, string phoneNumber)
        {
            try
            {
                var notificationId = await _smsClient.SendSmsAsync(Guid.NewGuid(), sendingService, phoneNumber);
                return notificationId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Send Sms Error");
                return Result.Failure<Guid>(SmsClientError.SendSMS);
            }
        }
    }
}