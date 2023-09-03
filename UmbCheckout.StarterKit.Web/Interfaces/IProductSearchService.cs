using UmbCheckout.StarterKit.Web.Models.Search;

namespace UmbCheckout.StarterKit.Web.Interfaces
{
    public interface IProductSearchService
    {
        ProductSearchResponse SearchProducts(ProductSearchCriteria criteria);
    }
}
