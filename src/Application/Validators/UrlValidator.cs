using System.Text.RegularExpressions;

namespace Application.Validators;

public static class UrlValidator
{
    private static readonly Regex UrlPattern = new(
        @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        if (Uri.TryCreate(url, UriKind.Absolute, out var result))
        {
            return result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps;
        }

        return false;
    }
}

