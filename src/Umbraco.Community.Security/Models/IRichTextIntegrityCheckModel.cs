namespace Umbraco.Community.Security.Models;

public interface IRichTextIntegrityCheckModel
{
    string Value { get; set; }

    string Alias { get; set; }

    int LanguageId { get; set; }

    string Segment { get; set; }

    Guid UniqueId { get; set; }

    string DocumentName { get; set; }
}