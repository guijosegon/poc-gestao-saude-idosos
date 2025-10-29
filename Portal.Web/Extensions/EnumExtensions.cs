using System.ComponentModel.DataAnnotations;

namespace GestaoSaudeIdosos.Web.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName<TEnum>(this TEnum value) where TEnum : struct, Enum
        {
            var member = typeof(TEnum).GetMember(value.ToString()).FirstOrDefault();
            var display = member?
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .OfType<DisplayAttribute>()
                .FirstOrDefault();

            return display?.Name ?? value.ToString();
        }

        public static IEnumerable<string> GetDisplayNames<TEnum>(this IEnumerable<TEnum> values) where TEnum : struct, Enum
        {
            return values.Select(GetDisplayName);
        }
    }
}