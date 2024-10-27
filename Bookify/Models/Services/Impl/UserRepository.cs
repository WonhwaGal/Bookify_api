using Bookify.Domain;
using Bookify.Identity.Services;
using Bookify.Infrastructure;
using Bookify.Models.Requests;
using Bookify.Models.Results;
using Bookify.Services;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Bookify.Models.Services.Impl
{
    public class UserRepository(
        ApplicationDbContext dbContext,
        ICacheService cacheService,
        IAccountRepository accountRepository) : IUserRepository
    {
        public User? GetById(Guid id)
        {
            return dbContext.Users.FirstOrDefault(user => user.Id == id);
        }

        public Result<User> GetByIdentityId(string? identityId)
        {
            if(identityId == null)
            {
                return Result.Failure<User>(UserError.EmptyData);
            }

            var user = dbContext.Users.FirstOrDefault(user => user.IdentityId == identityId);
            if(user == null)
            {
                return Result.Failure<User>(UserError.NotFound);
            }

            return user;
        }

        public async Task<Result<Guid>> AddUser(RegisterUserRequest request)
        {
            var userIdentityId = await accountRepository.CreateAccountAsync(
                request.FirstName!,
                request.LastName!,
                request.Email!,
                request.Password!
                );

            if (userIdentityId.IsFailure)
            {
                return Result.Failure<Guid>(UserError.AccountCreationFailed);
            }

            var newUser = User.Create(
                request.FirstName!, 
                request.LastName!, 
                request.Email!, 
                userIdentityId.Value);

            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();

            return newUser.Id;
        }

        public async Task<Result<string>> LogInUser(LoginRequest request)
        {
            var cacheKey = $"login-{request.Email}-{request.Password}";
            var cacheData = await cacheService.GetAsync<Result<string>>(cacheKey);
            if(cacheData is not null)
            {
                return cacheData;
            }

            var result = await accountRepository.LoginAccountAsync(
                request.Email!,
                request.Password!);

            await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));

            return result;
        }

        public int Create(User item)
        {
            throw new NotImplementedException();
        }

        public ICollection<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public int Remove(Guid id)
        {
            throw new NotImplementedException();
        }

        public int Update(User item)
        {
            throw new NotImplementedException();
        }
    }
}
