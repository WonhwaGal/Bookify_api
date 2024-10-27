using Bookify.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Models.Services
{
    public interface IDefaultSmsClient
    {
        Task<Result<Guid>> SendSmsAsync(Guid reviewId, string from, string phoneNumber);
    }
}
