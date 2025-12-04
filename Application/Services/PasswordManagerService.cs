using MassAidVOne.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace MassAidVOne.Application.Services
{
    public class PasswordManagerService : IPasswordManagerService
    {
        private readonly IPasswordHasher<Object> _passwordHasher;

        public PasswordManagerService(IPasswordHasher<object> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public string HashedPassword(string Password)
        {
            var hashedPassword = _passwordHasher.HashPassword(null, Password);
            return hashedPassword;
        }

        public bool VerifyPassword(UserInformation User, string EnteredPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, User.UserPassword, EnteredPassword);

            return result == PasswordVerificationResult.Success;
        }
    }
}
