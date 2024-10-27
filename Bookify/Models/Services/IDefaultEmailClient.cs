using Bookify.Domain;

namespace Bookify.Models.Services
{
    public interface IDefaultEmailClient
    {
        public Task<Result<Guid>> ForwardEmailAsync(Guid id, string from, string to, string subject, string body);
    }
}
