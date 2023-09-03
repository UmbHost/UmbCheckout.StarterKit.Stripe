using UmbCheckout.StarterKit.Web.NotificationHandlers;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;

namespace UmbCheckout.StarterKit.Web.Composers
{
    public class RegisterNotificationHandlersComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<ContentDeletedNotification, OnContentDeletedNotificationHandler>();
            builder.AddNotificationHandler<ContentMovedToRecycleBinNotification, OnContentMovedToRecycleBinNotification>();
            builder.AddNotificationHandler<UmbracoApplicationStartingNotification, TransformExamineValues>();
            builder.AddNotificationHandler<UmbracoApplicationStartingNotification, CreateBundlesNotificationHandler>();
        }
    }
}
