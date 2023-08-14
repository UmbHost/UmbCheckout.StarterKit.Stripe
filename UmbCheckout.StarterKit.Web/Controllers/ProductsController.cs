using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using UmbCheckout.StarterKit.Web.ViewModels;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace UmbCheckout.StarterKit.Web.Controllers
{
    public class ProductsController : RenderController
    {
        public ProductsController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor) 
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
        }

        public IActionResult Products(string category)
        {
            IEnumerable<IPublishedContent> products;
            if (!string.IsNullOrEmpty(category))
            {
                products = CurrentPage.Children()
                    .Where(x => x.Value<IEnumerable<IPublishedContent>>("categories").Select(x => x.Name)
                    .Contains(category, StringComparer.CurrentCultureIgnoreCase))
                    .Take(CurrentPage.Value<int>("maximum"));
            }
            else
            {
                products = CurrentPage.Children().Take(CurrentPage.Value<int>("maximum"));
            }

            var model = new ProductsViewModel(CurrentPage)
            {
                Products = products
            };

            return CurrentTemplate(model);
        }
    }
}
