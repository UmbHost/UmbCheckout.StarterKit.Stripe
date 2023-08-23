using Examine;
using Examine.Search;
using UmbCheckout.StarterKit.Web.Interfaces;
using UmbCheckout.StarterKit.Web.Models.Search;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Web.Common;

namespace UmbCheckout.StarterKit.Web.Services.Search
{
    internal class BlogSearchService : IBlogSearchService
    {
        private readonly IExamineManager _examineManager;
        private readonly UmbracoHelper _umbracoHelper;
        private readonly IShortStringHelper _shortStringHelper;

        public BlogSearchService(IExamineManager examineManager, UmbracoHelper umbracoHelper, IShortStringHelper shortStringHelper)
        {
            _examineManager = examineManager;
            _umbracoHelper = umbracoHelper;
            _shortStringHelper = shortStringHelper;
        }

        public BlogSearchResponse SearchProducts(BlogSearchCriteria criteria)
        {
            var response = new BlogSearchResponse(criteria);

            response.SearchResults = SearchUsingExamine(criteria);

            return response;
        }

        private SearchResults? SearchUsingExamine(BlogSearchCriteria criteria)
        {
            var results = new SearchResults();
            if (_examineManager.TryGetIndex(Umbraco.Cms.Core.Constants.UmbracoIndexes.ExternalIndexName, out var index))
            {
                var fieldsToSearch = new[]
                {
                    "nodeName",
                    "categoryNames",
                    "description",
                    "categories",
                    "detailedDescription"
                };

                var query = index.Searcher.CreateQuery("content").NodeTypeAlias("blogPost");

                if (!string.IsNullOrEmpty(criteria.Keywords))
                {
                    query.And().GroupedOr(fieldsToSearch, criteria.Keywords.MultipleCharacterWildcard());
                }

                if (!string.IsNullOrEmpty(criteria.Category))
                {
                    query.And().Field("categoryNames", criteria.Category.ToUrlSegment(_shortStringHelper));
                }


                string stringToParse = query.ToString();
                int indexOfPropertyValue = stringToParse.IndexOf("LuceneQuery:") + 12;
                string rawQuery = stringToParse.Substring(indexOfPropertyValue).TrimEnd('}');
                var response = index.Searcher.CreateQuery("content").NativeQuery(rawQuery).Execute(QueryOptions.SkipTake((criteria.CurrentPage - 1) * criteria.PageSize, criteria.PageSize));
                var searchResults = new List<IPublishedContent>();

                foreach (var id in response.Select(x => x.Id))
                {
                    searchResults.Add(_umbracoHelper.Content(id));
                }
                results.Items = searchResults;
                results.TotalResults = response.TotalItemCount;
            }

            return results;
        }
    }
}
