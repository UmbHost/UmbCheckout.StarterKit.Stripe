using System.ServiceModel.Syndication;

namespace UmbCheckout.StarterKit.Web.Interfaces
{
    public interface ISyndicationXmlService
    {
        Atom10FeedFormatter GenerateAtomXml(string title, string description);
        Rss20FeedFormatter GenerateRssXml(string title, string description);
    }
}