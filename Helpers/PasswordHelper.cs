using System;
using System.Security.Cryptography;

namespace Epros_CareerHubAPI.Helpers
{
    public interface IPasswordHelper
    {
        string CreateHash(string password);
        bool VerifyHash(string password, string storedHash);
    }

    public class PasswordHelper : IPasswordHelper
    {
        private const int SaltSize = 32; // 256 bit
        private const int KeySize = 32; // 256 bit
        private const int Iterations = 100_000;

        // Returns stored value in format: {saltBase64}:{hashBase64}
        public string CreateHash(string password)
        {
            if (password is null) throw new ArgumentNullException(nameof(password));

            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key = deriveBytes.GetBytes(KeySize);

            var saltB64 = Convert.ToBase64String(salt);
            var keyB64 = Convert.ToBase64String(key);

            return $"{saltB64}:{keyB64}";
        }

        public bool VerifyHash(string password, string storedHash)
        {
            if (password is null) throw new ArgumentNullException(nameof(password));
            if (storedHash is null) throw new ArgumentNullException(nameof(storedHash));

            var parts = storedHash.Split(':', 2);
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var expectedKey = Convert.FromBase64String(parts[1]);

            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key = deriveBytes.GetBytes(KeySize);

            return CryptographicOperations.FixedTimeEquals(key, expectedKey);
        }
    }
}