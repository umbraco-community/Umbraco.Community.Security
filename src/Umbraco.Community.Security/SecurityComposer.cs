using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Community.Security.Repositories;

namespace Umbraco.Community.Security;

public class SecurityComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<IRichTextIntegrityCheckRepository, RichTextIntegrityCheckRepository>();
    }
}