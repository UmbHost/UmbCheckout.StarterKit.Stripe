using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Community.Contentment.DataEditors;

namespace UmbCheckout.StarterKit.Web.Controllers
{
    public class FormController : SurfaceController
    {
        public FormController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitForm()
        {
            var formCollection = HttpContext.Request.Form;

            if (!formCollection.ContainsKey("formId") && Guid.TryParse(formCollection["formId"], out var formId))
            {
                ModelState.AddModelError(string.Empty, "The form id is invalid");
            }

            var contentBlocks = CurrentPage.Value<BlockListModel>("contentBlocks");
            if (contentBlocks != null)
            {
                var form = contentBlocks.FirstOrDefault(x => x.ContentUdi == Udi.Create("element", Guid.Parse(formCollection["formId"])));

                if (form != null)
                {
                    foreach (var formFieldItem in form.Content.Value<IEnumerable<BlockListItem>>("formFields"))
                    {
                        foreach (var formField in formCollection)
                        {
                            if (Guid.TryParse(formField.Key, out var fieldKey) && formFieldItem.Content.Key == fieldKey)
                            {
                                if (formFieldItem.Content.Value<bool>("required") &&
                                    string.IsNullOrEmpty(formField.Value))
                                {
                                    ModelState.AddModelError(string.Empty, formFieldItem.Content.Value<string>("validationErrorMessage"));
                                }

                                if (formFieldItem.Content.Value<bool>("validation"))
                                {
                                    if (formFieldItem.Content.Value<string>("validationType") == "customValidation" &&
                                        !string.IsNullOrEmpty(
                                            formFieldItem.Content.Value<string>("customValidationRegex")) &&
                                        !Regex.IsMatch(formField.Value, formFieldItem.Content.Value<string>("customValidationRegex")))
                                    {
                                        ModelState.AddModelError(string.Empty, formFieldItem.Content.Value<string>("validationErrorMessage"));
                                    }
                                    else if (!Regex.IsMatch(formField.Value,
                                                 formFieldItem.Content.Value<string>("validationType")))
                                    {
                                        ModelState.AddModelError(string.Empty, formFieldItem.Content.Value<string>("validationErrorMessage"));
                                    }
                                }
                            }
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        return RedirectToUmbracoPage(form.Content.Value<IPublishedContent>("confirmationPage"));
                    }
                }

                ModelState.AddModelError(string.Empty, "An error occurred trying to submit the form");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred trying to submit the form");
            }

            return CurrentUmbracoPage();
        }
    }
}
