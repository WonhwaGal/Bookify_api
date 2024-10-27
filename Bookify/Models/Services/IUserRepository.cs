using Bookify.Domain;
using Bookify.Models.Requests;

namespace Bookify.Models.Services
{
    public interface IUserRepository : IRepository<User>
    {
        Result<User> GetByIdentityId(string? identityId);

        Task<Result<Guid>> AddUser(RegisterUserRequest request);

        Task<Result<string>> LogInUser(LoginRequest request);
    }
}
