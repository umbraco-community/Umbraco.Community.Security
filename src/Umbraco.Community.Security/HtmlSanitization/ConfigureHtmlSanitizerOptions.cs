using AngleSharp.Css.Dom;
using Ganss.Xss;
using Microsoft.Extensions.Options;

namespace Umbraco.Community.Security.HtmlSanitization;

public class ConfigureHtmlSanitizerOptions : IConfigureOptions<HtmlSanitizerOptions>
{
    public void Configure(HtmlSanitizerOptions options)
    {
        options.AllowedTags = new HashSet<string>(HtmlSanitizerDefaults.AllowedTags, StringComparer.OrdinalIgnoreCase);
        options.AllowedSchemes = new HashSet<string>(HtmlSanitizerDefaults.AllowedSchemes, StringComparer.OrdinalIgnoreCase);
        options.AllowedAttributes = new HashSet<string>(HtmlSanitizerDefaults.AllowedAttributes, StringComparer.OrdinalIgnoreCase);
        options.UriAttributes = new HashSet<string>(HtmlSanitizerDefaults.UriAttributes, StringComparer.OrdinalIgnoreCase);
        options.AllowedCssProperties = new HashSet<string>(HtmlSanitizerDefaults.AllowedCssProperties, StringComparer.OrdinalIgnoreCase);
        options.AllowedAtRules = new HashSet<CssRuleType>(HtmlSanitizerDefaults.AllowedAtRules);
        options.AllowedCssClasses = new HashSet<string>(HtmlSanitizerDefaults.AllowedClasses);
        
        // Umbraco custom
        options.AllowedSchemes.Add("mailto");
    }
}