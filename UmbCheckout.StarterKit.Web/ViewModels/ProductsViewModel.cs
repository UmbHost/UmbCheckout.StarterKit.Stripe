using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbCheckout.StarterKit.Web.ViewModels
{
    public class ProductsViewModel : ContentModel
    {
        public ProductsViewModel(IPublishedContent? content) : base(content)
        {
        }

        public IEnumerable<IPublishedContent>? Products { get; set; }
    }
}
