using Ganss.Xss;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Composing;

namespace Umbraco.Community.Sustainability.TestSite;

public class ExampleComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.ConfigureOptions<ExamplePostConfigureHtmlSanitizerOptions>();
    }
    
    private class ExamplePostConfigureHtmlSanitizerOptions : IPostConfigureOptions<HtmlSanitizerOptions>
    {
        public void PostConfigure(string? name, HtmlSanitizerOptions options)
        {
            options.AllowedTags.Add("b");
        }
    }
}

