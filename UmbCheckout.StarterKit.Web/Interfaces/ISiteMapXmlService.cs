namespace UmbCheckout.StarterKit.Web.Interfaces
{
    public interface ISiteMapXmlService
    {
        string GenerateXml(Guid startNode, bool includeSelf = true);
    }
}