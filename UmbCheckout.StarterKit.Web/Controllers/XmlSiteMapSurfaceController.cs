using System.Text;
using Microsoft.AspNetCore.Mvc;
using UmbCheckout.StarterKit.Web.Extensions;
using UmbCheckout.StarterKit.Web.Interfaces;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace UmbCheckout.StarterKit.Web.Controllers
{
    public class XmlSiteMapSurfaceController : SurfaceController
    {
        private readonly ISiteMapXmlService _xmlSiteMapXmlService;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        public XmlSiteMapSurfaceController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, ISiteMapXmlService xmlSiteMapXmlService, IUmbracoContextFactory umbracoContextFactory)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _xmlSiteMapXmlService = xmlSiteMapXmlService;
            _umbracoContextFactory = umbracoContextFactory;
        }

        [Route("sitemap.xml")]
        [ResponseCache(Duration = 900, VaryByHeader = "Host")]
        public ContentResult SiteMap()
        {
            using var context = _umbracoContextFactory.EnsureUmbracoContext().UmbracoContext;
            var rootNode = context.Content.GetAtRoot()
                .FirstOrDefault(x => x.ContentType.Alias == "home");
            return Content(_xmlSiteMapXmlService.GenerateXml(rootNode.Key), "text/xml", Encoding.UTF8);
        }

        [Route("products.xml")]
        [ResponseCache(Duration = 900, VaryByHeader = "Host")]
        public ContentResult HomesSiteMap()
        {
            using var context = _umbracoContextFactory.EnsureUmbracoContext().UmbracoContext;
            var rootNode = context.Content.GetAtRoot()
                .FirstOrDefault(x => x.ContentType.Alias == "home").GetProductsPage();
            return Content(_xmlSiteMapXmlService.GenerateXml(rootNode.Key, false), "text/xml", Encoding.UTF8);
        }

        [Route("blogposts.xml")]
        [ResponseCache(Duration = 900, VaryByHeader = "Host")]
        public ContentResult ParksSiteMap()
        {
            using var context = _umbracoContextFactory.EnsureUmbracoContext().UmbracoContext;
            var rootNode = context.Content.GetAtRoot()
                .FirstOrDefault(x => x.ContentType.Alias == "home").GetBlogPage();
            return Content(_xmlSiteMapXmlService.GenerateXml(rootNode.Key, false), "text/xml", Encoding.UTF8);
        }
    }
}