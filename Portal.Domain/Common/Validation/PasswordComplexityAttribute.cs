using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoSaudeIdosos.Domain.Common.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class PasswordComplexityAttribute : ValidationAttribute
    {
        public int MinimumLength { get; set; } = PasswordPolicy.MinimumLength;

        public PasswordComplexityAttribute()
            : base(PasswordPolicy.BuildRequirementDescription())
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;

            if (value is not string password)
                return new ValidationResult(PasswordPolicy.BuildRequirementDescription(MinimumLength));

            if (string.IsNullOrEmpty(password))
                return ValidationResult.Success;

            if (!PasswordPolicy.IsValid(password, MinimumLength))
            {
                var message = ErrorMessage ?? PasswordPolicy.BuildRequirementDescription(MinimumLength);
                return new ValidationResult(message);
            }

            return ValidationResult.Success;
        }
    }
}
