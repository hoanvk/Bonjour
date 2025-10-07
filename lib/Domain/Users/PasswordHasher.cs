using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Bonjour.Domain.Users;

public class PasswordHasher
{
    public (string HashedPassword, string Salt) HashPassword(string password)
    {
        // Generate a 128-bit salt
        byte[] saltBytes = RandomNumberGenerator.GetBytes(128 / 8);
        string salt = Convert.ToBase64String(saltBytes);

        // Derive a 256-bit subkey (hashed password) using PBKDF2
        string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000, // Recommended iteration count
            numBytesRequested: 256 / 8)); // 256 bits / 8 = 32 bytes

        return (hashedPassword, salt);
    }

    public bool VerifyPassword(string password, string storedHashedPassword, string storedSalt)
    {
        byte[] saltBytes = Convert.FromBase64String(storedSalt);

        // Hash the provided password with the stored salt
        string hashedPasswordToVerify = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        // Compare the newly generated hash with the stored hash
        return hashedPasswordToVerify == storedHashedPassword;
    }
}