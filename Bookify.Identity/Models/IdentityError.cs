using Bookify.Domain;

namespace Bookify.Identity.Models
{
    public static class IdentityError
    {
        public static Error EmptyData => new (
            "IdentityError.EmptyData", 
            "Data should not be empty or null");

        public static Error UserExists => new(
            "IdentityError.UserExists",
            "User with specified parameters already exists");

        public static Error UserNotFound => new(
            "IdentityError.UserNotFound",
            "User with specified parameters was not found");

        public static Error UserCreateFailed => new(
            "IdentityError.UserCreateFailed",
            "User creation was failed");

        public static Error IncorrectPassword => new(
            "IdentityError.IncorrectPassword",
            "The password is incorrect");
    }
}
