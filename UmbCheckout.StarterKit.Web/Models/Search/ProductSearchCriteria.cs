namespace UmbCheckout.StarterKit.Web.Models.Search
{
    public class ProductSearchCriteria
    {
        public string? Keywords { get; set; }

        public string? Category { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 9;


    }
}
