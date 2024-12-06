using NPoco;
using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Dtos;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Community.Security.Models;
using Umbraco.Extensions;

namespace Umbraco.Community.Security.Repositories;

internal class RichTextIntegrityCheckRepository : IRichTextIntegrityCheckRepository
    {
        private readonly IScopeAccessor _scopeAccessor;

        public RichTextIntegrityCheckRepository(IScopeAccessor scopeAccessor)
        {
            _scopeAccessor = scopeAccessor;
        }

        private IUmbracoDatabase Database
        {
            get
            {
                if (_scopeAccessor.AmbientScope is null)
                {
                    throw new NotSupportedException("Need to be executed in a scope");
                }

                return _scopeAccessor.AmbientScope.Database;
            }
        }


        public async Task<IEnumerable<IRichTextIntegrityCheckModel>> GetDataAsync(CancellationToken cancellationToken)
        {
            // Gets the values of the current active rich text editors.
            Sql<ISqlContext> sql = Database.SqlContext.Sql()
                .Select(
                    $"[pd].[textValue] AS {nameof(RichTextIntegrityCheckProjectionDto.Value)}",
                    $"[pt].[alias] AS {nameof(RichTextIntegrityCheckProjectionDto.Alias)}",
                    $"[pd].[languageId] AS {nameof(RichTextIntegrityCheckProjectionDto.LanguageId)}",
                    $"[pd].[segment] AS {nameof(RichTextIntegrityCheckProjectionDto.Segment)}",
                    $"[n].[uniqueId] AS {nameof(RichTextIntegrityCheckProjectionDto.UniqueId)}",
                    $"[n].[text] AS {nameof(RichTextIntegrityCheckProjectionDto.DocumentName)}")
                .From<MyPropertyDataDto>("pd") // Get all property data
                .InnerJoin<MyPropertyTypeDto>("pt")
                    .On<MyPropertyDataDto, MyPropertyTypeDto>((pd, pt) => pd.PropertyTypeId == pt.Id, "pd", "pt") // For the property type
                .InnerJoin<DataTypeDto>("dt")
                    .On<MyPropertyTypeDto, DataTypeDto>((pt, dt) => pt.DataTypeId == dt.NodeId && dt.EditorAlias == Constants.PropertyEditors.Aliases.RichText, "pt", "dt") // Limit to only rich text editors
                .InnerJoin<ContentVersionDto>("cv")
                    .On<ContentVersionDto, MyPropertyDataDto>((cv, pd) => cv.Current == true && cv.Id == pd.VersionId, "cv", "pd") // Only the current version
                .InnerJoin<NodeDto>("n")
                .On<NodeDto, ContentVersionDto>((n, cv) => n.NodeId == cv.NodeId, "n", "cv"); // Get the document node

            var result = await Database.FetchAsync<RichTextIntegrityCheckProjectionDto>(sql);

            return result;
        }

        
        // Due to internal classes, we need to create these classes to map the data.
        [TableName(Constants.DatabaseSchema.Tables.PropertyType)]
        private class MyPropertyTypeDto
        {
            [Column("dataTypeId")]
            public int DataTypeId { get; set;}
            
            [Column("id")]
            public int Id { get; set;}
        }

        // Due to internal classes, we need to create these classes to map the data.
        [TableName(Constants.DatabaseSchema.Tables.PropertyData)]
        private class MyPropertyDataDto
        {
            [Column("propertyTypeId")]
            public int PropertyTypeId { get; set; }
            
            [Column("versionId")]
            public int VersionId { get; set;}
        }

        private class RichTextIntegrityCheckProjectionDto : IRichTextIntegrityCheckModel
        {
            public string Value { get; set; } = string.Empty;

            public string Alias { get; set; } = string.Empty;

            public int LanguageId { get; set; }

            public string Segment { get; set; } = string.Empty;

            public Guid UniqueId { get; set; }

            public string DocumentName { get; set; } = string.Empty;
        }
    }