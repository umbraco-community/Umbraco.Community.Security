using System.Text;
using Umbraco.Cms.Core.HealthChecks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Community.Security.Models;
using Umbraco.Community.Security.Repositories;

namespace Umbraco.Community.Security.HealthChecks;


/// <summary>
///     Health check for the integrity of the data in the database.
/// </summary>
[HealthCheck(
    "FF140BC8-8F98-4DA4-85C8-8F7135A849F5",
    "Rich Text Data Integrity Check",
    Description = "Checks that all values of rich text editors will be saved as the same value, using the current implementation of IHtmlSanitizer.",
    Group = "Data Integrity")]
public class RichTextIntegrityCheck : HealthCheck
{
    private readonly ICoreScopeProvider _coreScopeProvider;
    private readonly IRichTextIntegrityCheckRepository _richTextIntegrityCheckRepository;
    private readonly IHtmlSanitizer _htmlSanitizer;
    private readonly ILanguageService _languageService;

    public RichTextIntegrityCheck(ICoreScopeProvider coreScopeProvider, IRichTextIntegrityCheckRepository richTextIntegrityCheckRepository, IHtmlSanitizer htmlSanitizer, ILanguageService languageService)
    {
        _coreScopeProvider = coreScopeProvider;
        _richTextIntegrityCheckRepository = richTextIntegrityCheckRepository;
        _htmlSanitizer = htmlSanitizer;
        _languageService = languageService;
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<HealthCheckStatus>> GetStatus()
    {
        using ICoreScope scope = _coreScopeProvider.CreateCoreScope();
        IEnumerable<IRichTextIntegrityCheckModel> allRichTextContent = await _richTextIntegrityCheckRepository.GetDataAsync(CancellationToken.None);

        var propertyAliasWithConflict = new List<IRichTextIntegrityCheckModel>();
        Parallel.ForEach(allRichTextContent, richTextContent =>
        {
            var sanitizedContent = _htmlSanitizer.Sanitize(richTextContent.Value);
            if (string.Equals(sanitizedContent, richTextContent.Value) is false)
            {
                propertyAliasWithConflict.Add(richTextContent);
            }
        });

        if (propertyAliasWithConflict.Count == 0)
        {
            return
            [
                new HealthCheckStatus("All rich text content is valid based on the current implementation of <code>IHtmlSanitizer</code>.")
                {
                    ResultType = StatusResultType.Success,
                }
            ];
        }

        return
        [
            new HealthCheckStatus(await GetReportAsync(propertyAliasWithConflict, CancellationToken.None))
            {
                ResultType = StatusResultType.Warning,
            },
        ];
    }

    private async Task<string> GetReportAsync(IList<IRichTextIntegrityCheckModel> conflictingRichTextValues, CancellationToken cancellationToken)
    {
        IEnumerable<ILanguage> languages = await _languageService.GetAllAsync();
        var languageDictionary = languages.ToDictionary(x => x.Id);

        IEnumerable<IGrouping<(Guid UniqueId, string DocumentName), IRichTextIntegrityCheckModel>> groupedByDocumentId = conflictingRichTextValues.GroupBy(x => (x.UniqueId, x.DocumentName));

        var sb = new StringBuilder();
        sb.AppendLine("<p>Rich text content with conflicts based on the current implementation of <code>IHtmlSanitizer</code>.</p>");
        sb.AppendLine("<ul>");
        foreach (IGrouping<(Guid UniqueId, string DocumentName), IRichTextIntegrityCheckModel> documentGroup in groupedByDocumentId)
        {
            sb.AppendLine("<li>");
            var link = $"/umbraco/section/content/workspace/document/edit/{documentGroup.Key.UniqueId}";
            sb.AppendLine($"<b><a href=\"{link}\">{documentGroup.Key.DocumentName}</a></b> has conflicts for the following properties and variants:");

            IEnumerable<IGrouping<string, IRichTextIntegrityCheckModel>> groupedByAlias = documentGroup.GroupBy(x => x.Alias);

            foreach (IGrouping<string, IRichTextIntegrityCheckModel> richTextIntegrityCheckModels in groupedByAlias)
            {
                var htmlLinks = new List<string>();

                foreach (var singleLanguage in richTextIntegrityCheckModels)
                {
                    languageDictionary.TryGetValue(singleLanguage.LanguageId, out ILanguage? language);

                    // TODO Maybe there is exist a less nasty hack to get the link to the content editor from the server.
                    htmlLinks.Add($"<a href=\"{link}/{language?.IsoCode}\">{language?.CultureName ?? "N/A"}</a>");
                }

                sb.AppendLine($"<ul><li><code>{richTextIntegrityCheckModels.Key}</code>: {string.Join(", ", htmlLinks)}</li></ul>");
            }

            sb.AppendLine("</li>");
        }

        sb.AppendLine("</ul>");
        return sb.ToString();
    }

    /// <inheritdoc />
    public override HealthCheckStatus ExecuteAction(HealthCheckAction action)
        => throw new NotSupportedException("Configuration cannot be automatically fixed.");
}
