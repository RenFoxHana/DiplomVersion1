using System.Security.Cryptography;
using System.Text;

namespace DiplomVersion1.Helper
{
    internal class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 20; 
        private const int Iterations = 10000; 

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize);

            byte[] hashWithSalt = new byte[SaltSize + HashSize];
            Buffer.BlockCopy(salt, 0, hashWithSalt, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, hashWithSalt, SaltSize, HashSize);

            return Convert.ToBase64String(hashWithSalt);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            byte[] hashWithSalt = Convert.FromBase64String(hashedPassword);

            byte[] salt = new byte[SaltSize];
            byte[] hash = new byte[HashSize];
            Buffer.BlockCopy(hashWithSalt, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(hashWithSalt, SaltSize, hash, 0, HashSize);

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize);

            return CryptographicOperations.FixedTimeEquals(inputHash, hash);
        }
    }
}
