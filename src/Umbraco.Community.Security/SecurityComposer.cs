using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Community.Security.HtmlSanitization;
using Umbraco.Community.Security.Repositories;
using Umbraco.Extensions;
using IHtmlSanitizer = Umbraco.Cms.Core.Security.IHtmlSanitizer;

namespace Umbraco.Community.Security;

public class SecurityComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<IRichTextIntegrityCheckRepository, RichTextIntegrityCheckRepository>();
        builder.Services.AddUnique<IHtmlSanitizer, CommunityHtmlSanitizer>();
        builder.Services.ConfigureOptions<ConfigureHtmlSanitizerOptions>();
    }
}