namespace GeolocationProject.Helperseed
{
    public static class SeedCountries
    {
        public static readonly Dictionary<string, string> CountryCodeToName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "US", "United States" },
        { "CA", "Canada" },
        { "GB", "United Kingdom" },
        { "FR", "France" },
        { "DE", "Germany" },
        { "JP", "Japan" },
        { "CN", "China" },
        { "IN", "India" },
        { "BR", "Brazil" },
        { "RU", "Russia" },
    };
    }
}
