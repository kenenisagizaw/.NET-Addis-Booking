// Path: Services/PasswordService.cs
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using AddisBookingAdmin.Models;

namespace AddisBookingAdmin.Services
{
    /// <summary>
    /// Provides password validation, hashing, and verification services.
    /// </summary>
    public class PasswordService
    {         
        /// <summary>
        /// Validates the password against security requirements.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <param name="error">Error message if validation fails.</param>
        /// <returns>True if valid, false otherwise.</returns>
        public bool ValidatePassword(string password, out string error)
        {
            error = string.Empty;
            // Check for minimum length and non-whitespace
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                error = "Password must be at least 6 characters long.";
                return false;
            }
            // Check for at least one uppercase letter
            if (!password.Any(char.IsUpper))
            {
                error = "Password must contain at least one uppercase letter.";
                return false;
            }
            // Check for at least one digit
            if (!password.Any(char.IsDigit))
            {
                error = "Password must contain at least one number.";
                return false;
            }
            // Check for at least one special character
            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                error = "Password must contain at least one special character.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Hashes the password and stores the hash and salt in the user object.
        /// </summary>
        /// <param name="user">The user object to store hash and salt.</param>
        /// <param name="password">The plain password to hash.</param>
        public void HashPassword(User user, string password)
        {
            // Generate a cryptographically secure random salt
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the password using PBKDF2 with HMACSHA256
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));

            // Store hash and salt in user object
            user.PasswordHash = hashed;
            user.PasswordSalt = Convert.ToBase64String(salt);
        }

        /// <summary>
        /// Verifies a password against the stored hash and salt in the user object.
        /// </summary>
        /// <param name="user">The user object containing hash and salt.</param>
        /// <param name="password">The password to verify.</param>
        /// <returns>True if password matches, false otherwise.</returns>
        public bool VerifyPassword(User user, string password)
        {
            // Retrieve salt from user object
            var salt = Convert.FromBase64String(user.PasswordSalt);
            // Hash the provided password with the same salt and parameters
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));

            // Compare the computed hash with the stored hash
            return hashed == user.PasswordHash;
        }
    }
}
