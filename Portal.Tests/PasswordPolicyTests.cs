using GestaoSaudeIdosos.Domain.Common.Validation;

namespace Portal.Tests
{
    public class PasswordPolicyTests
    {
        [Theory]
        [InlineData("SenhaForte1!")]
        [InlineData("Portal@2024")]
        [InlineData("Admin#123")] 
        public void PasswordPolicy_Should_Accept_Valid_Passwords(string password)
        {
            var isValid = PasswordPolicy.IsValid(password);

            Assert.True(isValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abcdefg!")] // Missing uppercase and digits
        [InlineData("ABCDEFG1")] // Missing lowercase and special characters
        [InlineData("Abcdefgh")] // Missing digits and special characters
        [InlineData("Abcdefg1")] // Missing special characters
        [InlineData("Abc!123")] // Too short
        public void PasswordPolicy_Should_Reject_Invalid_Passwords(string? password)
        {
            var isValid = PasswordPolicy.IsValid(password);

            Assert.False(isValid);
        }
    }
}
