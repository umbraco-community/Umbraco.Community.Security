using Umbraco.Community.Security.Models;

namespace Umbraco.Community.Security.Repositories;

public interface IRichTextIntegrityCheckRepository
{
    Task<IEnumerable<IRichTextIntegrityCheckModel>> GetDataAsync(CancellationToken cancellationToken);
}