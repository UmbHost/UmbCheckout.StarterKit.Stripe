using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbCheckout.StarterKit.Web.Models.Search
{
    public class SearchResults
    {
        public IEnumerable<IPublishedContent> Products { get; set; }
        public long TotalResults { get; set; }
    }
}