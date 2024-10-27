using Bookify.Domain;

namespace Bookify.Models.Results
{
    public static class UserError
    {
        public static Error EmptyData = new(
            "User.Empty",
            "Request received null reference");

        public static Error NotFound = new(
            "User.Found",
            "The user with the specified identifier was not found");

        public static Error AccountCreationFailed = new(
            "User.Account",
            "User account failed to be created");
    }
}
