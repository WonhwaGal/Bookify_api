using Bookify.Domain;

namespace Bookify.Identity.Services
{
    public interface IAccountRepository
    {
        Task<Result<string>> CreateAccountAsync(string firstName, string lastName, string email, string password);
        Task<Result<string>> LoginAccountAsync(string email, string password);
    }
}
