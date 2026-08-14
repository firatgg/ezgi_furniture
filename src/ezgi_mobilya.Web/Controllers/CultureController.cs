using ezgi_mobilya.Web.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace ezgi_mobilya.Controllers;

public class CultureController : Controller
{
    [HttpGet]
    public IActionResult Set(string culture, string? returnUrl = "/")
    {
        if (!AppCultures.IsSupported(culture))
        {
            culture = AppCultures.Default;
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                Path = "/",
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps
            });

        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            returnUrl = "/";
        }

        var fragment = string.Empty;
        var hashIndex = returnUrl.IndexOf('#');
        if (hashIndex >= 0)
        {
            fragment = returnUrl[hashIndex..];
            returnUrl = returnUrl[..hashIndex];
        }

        if (!Url.IsLocalUrl(returnUrl))
        {
            returnUrl = "/";
        }

        return LocalRedirect(returnUrl + fragment);
    }
}
