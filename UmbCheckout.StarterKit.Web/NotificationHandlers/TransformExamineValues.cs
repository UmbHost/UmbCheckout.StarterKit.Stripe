using Examine;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Core.Web;

namespace UmbCheckout.StarterKit.Web.NotificationHandlers
{
    public class TransformExamineValues : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IExamineManager _examineManager;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IShortStringHelper _shortStringHelper;
        public TransformExamineValues(IExamineManager examineManager, IUmbracoContextFactory umbracoContextFactory, IShortStringHelper shortStringHelper)
        {
            _examineManager = examineManager;
            _umbracoContextFactory = umbracoContextFactory;
            _shortStringHelper = shortStringHelper;
        }

        public void Handle(UmbracoApplicationStartingNotification notification)
        {
            if (_examineManager.TryGetIndex("ProductsIndex", out var index))
            {
                ((BaseIndexProvider)index).TransformingIndexValues += (object sender, IndexingItemEventArgs e) =>
                {
                    var values = e.ValueSet.Values.ToDictionary(x => x.Key, x => (IEnumerable<object>)x.Value);
                    if (e.ValueSet.ItemType.InvariantEquals("product"))
                    {
                        try
                        {
                            if (e.ValueSet.Values.ContainsKey("categories"))
                            {
                                if (e.ValueSet.GetValue("categories") is string categories)
                                {
                                    var categoriesStringArray = categories.Split(",");
                                    var categoryNames = new List<string>();
                                    using var ctx = _umbracoContextFactory.EnsureUmbracoContext();

                                    foreach (var cat in categoriesStringArray)
                                    {
                                        var category = ctx.UmbracoContext.Content.GetById(UdiParser.Parse(cat));

                                        if (category != null)
                                        {
                                            categoryNames.Add(category.Value<string>("categoryName").ToUrlSegment(_shortStringHelper));
                                        }
                                    }

                                    values.Add("categoryNames", new[] { string.Join(" ", categoryNames) });
                                }
                            }

                            e.SetValues(values);
                        }
                        catch (Exception exception)
                        {
                        }
                    }
                };
            }
        }
    }
}
