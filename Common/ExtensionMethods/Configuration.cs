using Microsoft.Extensions.Configuration;

namespace Common.ExtensionMethods
{
    public static class ConfigurationExtensions
    {
        public static bool TryGetValue(this IConfiguration configuration, string key, out string value)
        {
            value = configuration.GetSection(key).Value;
            return !string.IsNullOrEmpty(value);
        }

        public static bool TryGetValue(this IConfiguration configuration, string key, out int value, int defaultValue = default)
        {
            value = defaultValue;

            if (TryGetValue(configuration, key, out string v))
            {
                return int.TryParse(v, out value);
            }

            return false;
        }
    }
}
