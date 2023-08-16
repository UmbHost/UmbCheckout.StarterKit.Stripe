using Examine;
using UmbCheckout.StarterKit.Web.Indexes;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Infrastructure.Examine;

namespace UmbCheckout.StarterKit.Web.Composers
{
    public class RegisterIndexesComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddExamineLuceneIndex<ProductsIndex, ConfigurationEnabledDirectoryFactory>("ProductsIndex");
        }
    }
}
