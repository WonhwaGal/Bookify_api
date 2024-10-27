using System.IdentityModel.Tokens.Jwt;
using Bookify.Domain;
using Bookify.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Bookify.Identity.Services
{
    public class AccountRepository(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IConfiguration configuration
        ) : IAccountRepository
    {
        public async Task<Result<string>> CreateAccountAsync(string firstName, string lastName, string email, string password)
        {
            if(firstName == null || lastName == null || email == null || password == null)
            {
                return Result.Failure<string>(Models.IdentityError.EmptyData);
            }

            var newUser = new User
            {
                Email = email,
                UserName = firstName
            };

            var user = await userManager.FindByEmailAsync(newUser.Email);
            if(user is not null)
                return Result.Failure<string>(Models.IdentityError.UserExists);

            var createUserResult = await userManager.CreateAsync(newUser, password);
            if(!createUserResult.Succeeded)
                return Result.Failure<string>(Models.IdentityError.UserCreateFailed);

            await userManager.AddToRoleAsync(newUser, "Manager");

            return Result.Success(newUser.Id);
        }

        public async Task<Result<string>> LoginAccountAsync(string email, string password)
        {
            if (email == null || password == null)
            {
                return Result.Failure<string>(Models.IdentityError.EmptyData);
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return Result.Failure<string>(Models.IdentityError.UserNotFound);

            bool checkUserPasswords = await userManager.CheckPasswordAsync(user, password);
            if (!checkUserPasswords)
                return Result.Failure<string>(Models.IdentityError.IncorrectPassword);

            var roles = await userManager.GetRolesAsync(user);

            var userSession = new UserSession
            {
                Email = user.Email,
                Id = user.Id,
                Roles = roles.ToList()
            };

            var token = GenerateToken(userSession);
            return Result.Success(token);
        }

        private string GenerateToken(UserSession userSession)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userSession.Id),
                new Claim(ClaimTypes.Email, userSession.Email)
            };
            foreach (var role in userSession.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
