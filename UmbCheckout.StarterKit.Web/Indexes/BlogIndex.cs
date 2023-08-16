using Examine;
using Examine.Lucene;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;
using IHostingEnvironment = Umbraco.Cms.Core.Hosting.IHostingEnvironment;

namespace UmbCheckout.StarterKit.Web.Indexes
{
    public class ProductsIndex : UmbracoExamineIndex, IUmbracoContentIndex
    {
        private readonly string[] _contentTypes = {
            "product"
        };
        public ProductsIndex(
            ILoggerFactory loggerFactory,
            string name,
            IOptionsMonitor<LuceneDirectoryIndexOptions> indexOptions,
            IHostingEnvironment hostingEnvironment,
            IRuntimeState runtimeState)
            : base(loggerFactory, name, indexOptions, hostingEnvironment, runtimeState)
        {
            loggerFactory.CreateLogger<ProductsIndex>();

            LuceneDirectoryIndexOptions namedOptions = indexOptions.Get(name);
            if (namedOptions == null)
            {
                throw new InvalidOperationException($"No named {typeof(LuceneDirectoryIndexOptions)} options with name {name}");
            }

            namedOptions.FieldDefinitions.AddOrUpdate(new FieldDefinition("nodeName", FieldDefinitionTypes.FullTextSortable));

            if (namedOptions.Validator is IContentValueSetValidator contentValueSetValidator)
            {
                PublishedValuesOnly = contentValueSetValidator.PublishedValuesOnly;
            }

        }

        void IIndex.IndexItems(IEnumerable<ValueSet> values) => PerformIndexItems(values.Where(v => _contentTypes.Any(x => x == v.ItemType)), OnIndexOperationComplete);

    }
}