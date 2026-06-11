using System;

namespace SmartNovel.Helpers
{
    public static class DisplayHelpers
    {
        public static string FormatCompactNumber(int number)
        {
            if (number >= 1000000)
                return $"{number / 1000000.0:F1}M";

            if (number >= 1000)
                return $"{number / 1000.0:F1}K";

            return number.ToString();
        }
    }
}