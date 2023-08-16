using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbCheckout.StarterKit.Web.Models.Search
{
    public class SearchResults
    {
        public IEnumerable<IPublishedContent> Items { get; set; } = Enumerable.Empty<IPublishedContent>();
        public long TotalResults { get; set; }
    }
}