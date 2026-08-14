namespace ezgi_mobilya.Web.Localization;

public static class AppCultures
{
    public const string Default = "tr";

    public static readonly string[] Supported = ["tr", "en"];

    public static bool IsSupported(string? culture) =>
        !string.IsNullOrWhiteSpace(culture)
        && Supported.Contains(culture, StringComparer.OrdinalIgnoreCase);
}
