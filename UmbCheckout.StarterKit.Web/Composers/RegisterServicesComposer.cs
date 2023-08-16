using UmbCheckout.StarterKit.Web.Interfaces;
using UmbCheckout.StarterKit.Web.Services.Search;
using Umbraco.Cms.Core.Composing;

namespace UmbCheckout.StarterKit.Web.Composers
{
    public class RegisterServicesComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient<IProductSearchService, ProductSearchService>();
        }
    }
}
