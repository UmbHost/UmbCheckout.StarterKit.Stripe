using System.Web;

namespace UmbCheckout.StarterKit.Web.Extensions
{
    public static class StringExtensions
    {
        public static string ToRewrittenUrl(this string url, string linkType, string name)
        {
            return $"{url}/{linkType}/{HttpUtility.UrlEncode(name).ToLower()}";
        }
    }
}
