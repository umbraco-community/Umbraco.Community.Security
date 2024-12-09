using Ganss.Xss;
using Microsoft.Extensions.Options;

namespace Umbraco.Community.Security.HtmlSanitization;

public class CommunityHtmlSanitizer: Umbraco.Cms.Core.Security.IHtmlSanitizer
{
    private readonly HtmlSanitizer _sanitizer;

    public CommunityHtmlSanitizer(IOptions<HtmlSanitizerOptions> options)
    {
        _sanitizer = new HtmlSanitizer(options.Value);
    }

    public string Sanitize(string html)
    {
        var result = _sanitizer.Sanitize(html);
        
        return result;
    }
}