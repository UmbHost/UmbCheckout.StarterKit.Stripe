using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.Email;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace UmbCheckout.StarterKit.Web.Controllers
{
    public class FormController : SurfaceController
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<FormController> _logger;
        private readonly GlobalSettings _globalSettings;
        public FormController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, IEmailSender emailSender, ILogger<FormController> logger, IOptions<GlobalSettings> globalSettings)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _emailSender = emailSender;
            _logger = logger;
            _globalSettings = globalSettings.Value;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitForm()
        {
            var formCollection = HttpContext.Request.Form;

            if (!formCollection.ContainsKey("formId") && Guid.TryParse(formCollection["formId"], out var formId))
            {
                ModelState.AddModelError(string.Empty, "The form id is invalid");
            }

            if (formCollection.ContainsKey("031660d1657942ba8675daf94f016b6e"))
            {
                var honeyPotField = formCollection["031660d1657942ba8675daf94f016b6e"];
                if (!string.IsNullOrEmpty(honeyPotField))
                {
                    ModelState.AddModelError(string.Empty, "An error occurred trying to submit the form");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred trying to submit the form");
            }

            if (formCollection.ContainsKey("085e5604d16a4719ba3a4415beb1bcea"))
            {
                var checkField = formCollection["085e5604d16a4719ba3a4415beb1bcea"];
                var dateViewed = new DateTime(Convert.ToInt64(checkField));
                var difference = DateTime.Now - dateViewed;

                if (checkField == 0 || difference.TotalSeconds < 10)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred trying to submit the form");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred trying to submit the form");
            }

            var contentBlocks = CurrentPage.Value<BlockListModel>("contentBlocks");
            if (contentBlocks != null)
            {
                var form = contentBlocks.FirstOrDefault(x => x.ContentUdi == Udi.Create("element", Guid.Parse(formCollection["formId"])));

                if (form != null)
                {
                    var formFields = form.Content.Value<IEnumerable<BlockListItem>>("formFields");
                    var formMessageField = form.Content.Value<IEnumerable<BlockListItem>>("message");
                    ValidateForm(formFields, formCollection);
                    ValidateForm(formMessageField, formCollection, true);

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            var messageBlock = formMessageField?.FirstOrDefault();
                            var messageKey = messageBlock.Content.Key.ToString("N");
                            var formMessage = formCollection[messageKey];
                            var fromAddress = form.Content.HasValue("fromEmailAddress") ? form.Content.Value<string>("fromEmailAddress") : _globalSettings.Smtp.From;

                            var subject = form.Content.Value<string>("subject");

                            var sb = new StringBuilder();
                            sb.AppendLine(form.Content.Value<string>("messageIntroduction"));
                            sb.AppendLine();

                            foreach (var formField in formCollection.Where(x => Guid.TryParse(x.Key, out _)))
                            {
                                if (formFields.Select(x => x.Content.Key).Any(x => x == Guid.Parse(formField.Key)))
                                {
                                    var label = formFields.FirstOrDefault(x => x.Content.Key == Guid.Parse(formField.Key))
                                        .Content.Value<string>("label");

                                    sb.AppendLine($"{label}: {formField.Value}");
                                }
                            }

                            sb.AppendLine($"{messageBlock.Content.Value<string>("label")}: {formMessage}");

                            EmailMessage message = new EmailMessage(fromAddress, form.Content.Value<string>("toEmailAddress"), subject, sb.ToString(), false);
                            await _emailSender.SendAsync(message, emailType: "Form Submission");
                            TempData["formSubmittedSuccessfully"] = true;

                            return RedirectToUmbracoPage(form.Content.Value<IPublishedContent>("confirmationPage"));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error When Submitting Contact Form");
                            ModelState.AddModelError(string.Empty, "An error occurred trying to submit the form");
                        }
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

        private void ValidateForm(IEnumerable<BlockListItem> formFields, IFormCollection formCollection, bool alwaysRequired = false)
        {
            foreach (var formFieldItem in formFields)
            {
                foreach (var formField in formCollection)
                {
                    if (Guid.TryParse(formField.Key, out var fieldKey) && formFieldItem.Content.Key == fieldKey)
                    {
                        if (!alwaysRequired && formFieldItem.Content.Value<bool>("required") &&
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
        }
    }
}
