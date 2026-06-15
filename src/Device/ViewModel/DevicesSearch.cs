using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace EasyEPlanner.Devices.ViewModel
{
    public static class DevicesSearch
    {
        public static bool Contains(string valueForSearch, string searchedValue)
        {
            if (!UseRegex && !SearchWholeWord)
            {
                return valueForSearch.IndexOf(searchedValue,
                    StringComparison.OrdinalIgnoreCase) >= 0;
            }

            if (!UseRegex)
            {
                try
                {
                    return Regex.IsMatch(valueForSearch, $"(^| ){searchedValue}($| )",
                        RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
                }
                catch
                {
                    return false;
                }
            }

            var pattern = string.Join("|", searchedValue
                .Split(["||"], StringSplitOptions.None)
                .Select(valueOR =>
                {
                    var valuesAND = valueOR.Trim()
                        .Split(["&&"], StringSplitOptions.RemoveEmptyEntries);

                    return string.Join("", valuesAND
                        .Select(value => SearchWholeWord
                            ? $"(?=.*(^| ){value.Trim()}($| ))"
                            : value.Trim()));
                }));

            try
            {
                return Regex.IsMatch(valueForSearch, pattern,
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
            }
            catch
            {
                return false;
            }
        }

        public static bool SearchWholeWord { get; set; }

        public static bool UseRegex { get; set; }
    }
}
