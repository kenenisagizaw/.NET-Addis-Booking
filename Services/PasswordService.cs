// Path: Services/PasswordService.cs
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using AddisBookingAdmin.Models;

namespace AddisBookingAdmin.Services
{
    public class PasswordService
    {         
        // Checks password strength and requirements
        public bool ValidatePassword(string password, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                error = "Password must be at least 6 characters long.";
                return false;
            }
            if (!password.Any(char.IsUpper))
            {
                error = "Password must contain at least one uppercase letter.";
                return false;
            }
            if (!password.Any(char.IsDigit))
            {
                error = "Password must contain at least one number.";
                return false;
            }
            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                error = "Password must contain at least one special character.";
                return false;
            }
            return true;
        }

        // Hashes password and stores salt
        public void HashPassword(User user, string password)
        {
            byte[] salt = new byte[128 / 8]; // Generate salt
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));

            user.PasswordHash = hashed;
            user.PasswordSalt = Convert.ToBase64String(salt);
        }

        // Verifies password against stored hash
        public bool VerifyPassword(User user, string password)
        {
            var salt = Convert.FromBase64String(user.PasswordSalt);
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));

            return hashed == user.PasswordHash;
        }
    }
}
