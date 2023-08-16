using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using UmbCheckout.StarterKit.Web.Extensions;
using UmbCheckout.StarterKit.Web.Interfaces;
using UmbCheckout.StarterKit.Web.Models.Search;
using UmbCheckout.StarterKit.Web.ViewModels;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace UmbCheckout.StarterKit.Web.Controllers
{
    public class BlogController : RenderController
    {
        private readonly IBlogSearchService _blogSearchService;
        private readonly IAppPolicyCache _cache;
        public BlogController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, AppCaches caches, IBlogSearchService blogSearchService)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _blogSearchService = blogSearchService;
            _cache = caches.RuntimeCache;
        }

        public IActionResult Blog(int page = 1, string? keywords = null, string? category = null)
        {
            var searchCriteria = new BlogSearchCriteria
            {
                Keywords = keywords,
                Category = category,
                CurrentPage = page,
                PageSize = CurrentPage.Value<int>("maximumPerPage")
            };

            var searchResults = _blogSearchService.SearchProducts(searchCriteria);

            var model = new BlogViewModel(CurrentPage)
            {
                SearchResponse = searchResults,
                Categories = GetBlogCategories()
            };

            return CurrentTemplate(model);
        }

        private IEnumerable<string?> GetBlogCategories()
        {
            return _cache.GetCacheItem("BlogCategories", () => CurrentPage.GetHomePage()
                    .FirstChildOfType("blogCategories")
                    .Children()
                    .Select(x => x.Value<string>("categoryName")),
                TimeSpan.FromMinutes(60)) ?? Enumerable.Empty<string?>();
        }
    }
}
