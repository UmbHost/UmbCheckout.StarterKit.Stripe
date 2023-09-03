using UmbCheckout.StarterKit.Web.Models.Search;

namespace UmbCheckout.StarterKit.Web.Interfaces
{
    public interface IBlogSearchService
    {
        BlogSearchResponse SearchProducts(BlogSearchCriteria criteria);
    }
}
