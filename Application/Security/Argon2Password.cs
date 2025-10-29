using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace GestaoSaudeIdosos.Application.Security
{
    public class Argon2PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int MemorySize = 131072;
        private const int Iterations = 3;

        public string Hash(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var bytes = Encoding.UTF8.GetBytes(password);
            var degreeOfParallelism = Math.Max(2, Environment.ProcessorCount);

            using var a2 = new Argon2id(bytes)
            {
                Salt = salt,
                MemorySize = MemorySize,
                Iterations = Iterations,
                DegreeOfParallelism = degreeOfParallelism
            };

            var hash = a2.GetBytes(HashSize);
            return "$argon2id$v=19$m=" + MemorySize + ",t=" + Iterations + ",p=" + degreeOfParallelism + "$"
                + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
        }

        public bool Verify(string hashed, string password)
        {
            if (string.IsNullOrWhiteSpace(hashed) || string.IsNullOrEmpty(password))
                return false;

            var parts = hashed.Split('$', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 4)
                return false;

            var parameters = parts[2];
            var parameterValues = ParseParameters(parameters);

            if (!parameterValues.TryGetValue('m', out var memorySize))
                memorySize = MemorySize;
            if (!parameterValues.TryGetValue('t', out var iterations))
                iterations = Iterations;
            if (!parameterValues.TryGetValue('p', out var degree))
                degree = Math.Max(2, Environment.ProcessorCount);

            var salt = Convert.FromBase64String(parts[^2]);
            var expected = Convert.FromBase64String(parts[^1]);
            var bytes = Encoding.UTF8.GetBytes(password);

            using var a2 = new Argon2id(bytes)
            {
                Salt = salt,
                MemorySize = memorySize,
                Iterations = iterations,
                DegreeOfParallelism = degree
            };

            var actual = a2.GetBytes(expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }

        public bool IsHashed(string value) => !string.IsNullOrWhiteSpace(value) && value.StartsWith("$argon2id$", StringComparison.Ordinal);

        private static Dictionary<char, int> ParseParameters(string parameters)
        {
            var result = new Dictionary<char, int>();

            if (string.IsNullOrWhiteSpace(parameters))
                return result;

            var segments = parameters.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var segment in segments)
            {
                var parts = segment.Split('=', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    continue;

                if (parts[0].Length != 1)
                    continue;

                if (int.TryParse(parts[1], out var value))
                    result[parts[0][0]] = value;
            }

            return result;
        }
    }
}
