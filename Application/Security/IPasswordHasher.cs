namespace GestaoSaudeIdosos.Application.Security
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string hashed, string password);
        bool IsHashed(string value);
    }
}
