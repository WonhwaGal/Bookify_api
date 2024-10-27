using Bookify.Models.Abstractions;

namespace Bookify.Models
{
    public class User: Entity
    {
        public static User Create(string firstName, string lastName, string email, string identityId)
        {
            var user = new User(Guid.NewGuid(), firstName, lastName, email, identityId);
            return user;
        }

        private User(Guid id, string firstName, string lastName, string email, string identityId) : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            IdentityId = identityId;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string IdentityId { get; private set; }
    }
}