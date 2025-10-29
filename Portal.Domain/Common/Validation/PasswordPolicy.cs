namespace GestaoSaudeIdosos.Domain.Common.Validation
{
    public static class PasswordPolicy
    {
        public const int MinimumLength = 8;

        public static bool IsValid(string? password, int minimumLength = MinimumLength)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < minimumLength)
                return false;

            var hasUpper = password.Any(char.IsUpper);
            var hasLower = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);
            var hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        public static string BuildRequirementDescription(int minimumLength = MinimumLength)
        {
            return $"A senha deve conter pelo menos {minimumLength} caracteres, incluindo letras maiúsculas, letras minúsculas, números e caracteres especiais.";
        }
    }
}
