using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbCheckout.StarterKit.Web.ViewModels.ViewComponents
{
    public class RecentBlogPostsViewModel
    {
        public string Heading { get; set; } = "Recent Blog Posts";
        public IEnumerable<IPublishedContent>? BlogPosts { get; set; } = Enumerable.Empty<IPublishedContent>();
    }
}
