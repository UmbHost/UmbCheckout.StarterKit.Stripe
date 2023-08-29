using Microsoft.AspNetCore.Mvc;
using UmbCheckout.StarterKit.Web.Extensions;
using UmbCheckout.StarterKit.Web.ViewModels.ViewComponents;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UmbCheckout.StarterKit.Web.ViewComponents.Blog
{
    public class BlogCategoriesViewComponent : ViewComponent
    {
        private readonly IAppPolicyCache _cache;

        public BlogCategoriesViewComponent(AppCaches caches)
        {
            _cache = caches.RuntimeCache;
        }

        public IViewComponentResult Invoke(IPublishedContent content, string heading = "Categories")
        {
            var model = new BlogCategoriesViewModel
            {
                Heading = heading,
                Categories = GetBlogCategories(content)
            };

            return View("~/Views/Components/BlogCategories/Default.cshtml", model);
        }

        private IEnumerable<string?> GetBlogCategories(IPublishedContent content)
        {
            return _cache.GetCacheItem("BlogCategories", () => content.GetHomePage()?
                    .FirstChildOfType("blogCategories")?
                    .Children()?
                    .Select(x => x.Value<string>("categoryName")),
                TimeSpan.FromMinutes(60)) ?? Enumerable.Empty<string?>();
        }
    }
}
