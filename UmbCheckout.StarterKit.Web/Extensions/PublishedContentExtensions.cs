using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbCheckout.StarterKit.Web.Extensions
{
    public static class PublishedContentExtensions
    {
        public static IPublishedContent? GetHomePage(this IPublishedContent content) => content.Root();

        public static IPublishedContent? GetSiteSettings(this IPublishedContent content) =>
            GetHomePage(content)?.FirstChildOfType("siteSettings");
    }
}
