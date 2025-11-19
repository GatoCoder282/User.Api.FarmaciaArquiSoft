using System;
using System.Security.Cryptography;
using System.Text;
using User.Domain.Ports;

namespace User.Application.Services
{
    public class PasswordService : IPasswordService
    {
        private const int Pbkdf2Iterations = 100_000;
        private const int SaltSize = 16;
        private const int KeySize = 32;

        public string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
#if NET6_0_OR_GREATER
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Pbkdf2Iterations, HashAlgorithmName.SHA256, KeySize);
#else
            using var derive = new Rfc2898DeriveBytes(password, salt, Pbkdf2Iterations);
            var hash = derive.GetBytes(KeySize);
#endif
            return $"PBKDF2|{Pbkdf2Iterations}|{Convert.ToBase64String(salt)}|{Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(string password, string stored)
        {
            var parts = stored.Split('|');
            if (parts.Length != 4 || parts[0] != "PBKDF2") return false;

            var iterations = int.Parse(parts[1]);
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
#if NET6_0_OR_GREATER
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
#else
            using var derive = new Rfc2898DeriveBytes(password, salt, iterations);
            var actual = derive.GetBytes(expected.Length);
#endif
            if (actual.Length != expected.Length) return false;
            var diff = 0;
            for (int i = 0; i < actual.Length; i++) diff |= actual[i] ^ expected[i];
            return diff == 0;
        }

        public string GenerateRandomPassword(int length)
        {
            if (length < 8) length = 8;
            const string lowers = "abcdefghijklmnopqrstuvwxyz";
            const string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string symbols = "!@#$%^&*_-";

            var required = new[]
            {
                lowers[RandomNumberGenerator.GetInt32(lowers.Length)],
                uppers[RandomNumberGenerator.GetInt32(uppers.Length)],
                digits[RandomNumberGenerator.GetInt32(digits.Length)],
                symbols[RandomNumberGenerator.GetInt32(symbols.Length)]
            }.ToList();

            string all = lowers + uppers + digits + symbols;
            while (required.Count < length)
                required.Add(all[RandomNumberGenerator.GetInt32(all.Length)]);

            for (int i = required.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (required[i], required[j]) = (required[j], required[i]);
            }
            return new string(required.ToArray());
        }
    }
}