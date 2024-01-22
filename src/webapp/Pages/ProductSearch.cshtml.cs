using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{
    public class ProductSearchModel : PageModel
    {
        [BindProperty]
        public string SearchKeywords { get; set; } = string.Empty;

        public SearchResults<Product>? SearchResults { get; set; }

        private readonly SearchIndexClient? indexClient;
        private readonly SearchClient? searchClient;

        public string IndexName { get; set; }
        public long DocumentCount { get; set; }

        public ProductSearchModel()
        {
            Uri endpoint = new Uri("https://search-aais-scratch-uksouth-3ad9c956.search.windows.net");

            // Create a client for manipulating search indexes
            indexClient = new SearchIndexClient(endpoint, new DefaultAzureCredential());

            if (indexClient.GetIndexes().All(a => a.Name != "products"))
            {
                // Create a new index
                indexClient.CreateIndex(new SearchIndex("products")
                {
                    Fields = new FieldBuilder().Build(typeof(Product))
                });
            }

            var searchIndex = indexClient.GetIndex("products");
            searchClient = indexClient.GetSearchClient("products");

            IndexName = "products";
            DocumentCount = searchClient.GetDocumentCount();
        }

        public void OnGet()
        {

        }

        public async Task OnPostGenerateCatalog()
        {
            await using SearchIndexingBufferedSender<Product> indexer =
                new SearchIndexingBufferedSender<Product>(searchClient);
            await indexer.UploadDocumentsAsync(GenerateCatalog(count: 2000));
        }

        public void OnPostSearch()
        {
            // Perform the search query
            SearchResults<Product> results = searchClient.Search<Product>(SearchKeywords, new SearchOptions
            {
                IncludeTotalCount = true,
                OrderBy = { "Price desc" },
                Skip = 0,
                Size = 10
            });

            // Store the search results
            SearchResults = results;
        }

        public void OnPostDeleteCatalog()
        {
            indexClient.DeleteIndex("products");

            indexClient.CreateIndex(new SearchIndex("products")
            {
                Fields = new FieldBuilder().Build(typeof(Product))
            });
        }

        public IEnumerable<Product> GenerateCatalog(int count = 1000)
        {
            // Adapted from https://weblogs.asp.net/dfindley/Microsoft-Product-Name-Generator
            var prefixes = new[] { null, "Visual", "Compact", "Embedded", "Expression" };
            var products = new[] { null, "Windows", "Office", "SQL", "FoxPro", "BizTalk" };
            var terms = new[] { "Web", "Robotics", "Network", "Testing", "Project", "Small Business", "Team", "Management", "Graphic", "Presentation", "Communication", "Workflow", "Ajax", "XML", "Content", "Source Control" };
            var type = new[] { null, "Client", "Workstation", "Server", "System", "Console", "Shell", "Designer" };
            var suffix = new[] { null, "Express", "Standard", "Professional", "Enterprise", "Ultimate", "Foundation", ".NET", "Framework" };
            var components = new[] { prefixes, products, terms, type, suffix };

            var random = new Random();
            string RandomElement(string[] values) => values[(int)(random.NextDouble() * values.Length)];
            double RandomPrice() => (random.Next(2, 20) * 100.0) / 2.0 - .01;

            for (int i = 1; i <= count; i++)
            {
                yield return new Product
                {
                    Id = i.ToString(),
                    Name = string.Join(" ", components.Select(RandomElement).Where(n => n != null)),
                    Price = RandomPrice()
                };
            }
        }
    }

    public class Product
    {
        [SimpleField(IsKey = true)]
        public string Id { get; set; }

        [SearchableField(IsFilterable = true)]
        public string Name { get; set; }

        [SimpleField(IsSortable = true)]
        public double Price { get; set; }

        public override string ToString() =>
            $"{Id}: {Name} for {Price:C}";
    }
}
