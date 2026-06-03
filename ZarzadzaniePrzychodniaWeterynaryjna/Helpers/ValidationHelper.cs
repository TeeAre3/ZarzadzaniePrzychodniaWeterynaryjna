using System.Text.RegularExpressions;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Helpers
{
    public static partial class ValidationHelper
    {
        private static readonly Regex EmailRegex = MyRegex();
        private static readonly Regex PhoneRegex = MyRegex1();
        private static readonly Regex TimeRegex = MyRegex2();

        public static bool IsValidEmail(string email) =>
            EmailRegex.IsMatch(email);

        public static bool IsValidPhone(string phone)
        {
            var cleanPhone = phone.Replace(" ", "").Replace("-", "");
            return PhoneRegex.IsMatch(cleanPhone);
        }

        public static bool IsValidTimeFormat(string time) =>
            TimeRegex.IsMatch(time);
        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
        private static partial Regex MyRegex();
        [GeneratedRegex(@"^\+?[0-9]{9,15}$", RegexOptions.Compiled)]
        private static partial Regex MyRegex1();
        [GeneratedRegex(@"^(0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", RegexOptions.Compiled)]
        private static partial Regex MyRegex2();
    }
}