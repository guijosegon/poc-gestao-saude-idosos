using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

public class Argon2PasswordHasher
{
    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var bytes = Encoding.UTF8.GetBytes(password);

        using var a2 = new Argon2id(bytes)
        {
            Salt = salt,
            MemorySize = 131072,
            Iterations = 3,
            DegreeOfParallelism = Math.Max(2, Environment.ProcessorCount)
        };

        var hash = a2.GetBytes(32);
        return $"$argon2id$v=19$m=131072,t=3,p={Environment.ProcessorCount}$" + $"{Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string hashed, string password)
    {
        var parts = hashed.Split('$', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 4) return false;

        var param = parts[1 + 1];
        var salt = Convert.FromBase64String(parts[^2]);
        var expected = Convert.FromBase64String(parts[^1]);

        var bytes = Encoding.UTF8.GetBytes(password);

        using var a2 = new Argon2id(bytes)
        {
            Salt = salt,
            MemorySize = 131072,
            Iterations = 3,
            DegreeOfParallelism = Math.Max(2, Environment.ProcessorCount)
        };

        var actual = a2.GetBytes(expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}