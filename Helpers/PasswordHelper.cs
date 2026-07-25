using System.Security.Cryptography;

namespace TravelCoServer.Helpers
{
    public static class PasswordHelper
    {
        public static (string hash, string salt) CreateHash(string password)
        {
            // 1. generate a random 16-byte salt
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            // 2. derive a 32-byte hash from (password + salt) using PBKDF2
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            // 3. return both as Base64 strings
            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }

        public static bool Verify(string password, string storedHash, string storedSalt)
        {
            // 1. Base64-decode the stored salt back to bytes
            byte[] salt = Convert.FromBase64String(storedSalt);

            // 2. re-derive the hash with the SAME salt + settings
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            string computed = Convert.ToBase64String(pbkdf2.GetBytes(32));

            // 3. return whether it equals storedHash
            return computed == storedHash;
        }
    }
}

