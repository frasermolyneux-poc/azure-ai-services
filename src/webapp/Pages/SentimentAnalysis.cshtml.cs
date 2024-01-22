using System.ComponentModel.DataAnnotations;
using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{
    public class SentimentAnalysisModel : PageModel
    {

        [BindProperty]
        [Required(ErrorMessage = "Please enter some text to analyse.")]
        [MinLength(1, ErrorMessage = "Please enter some text to analyse.")]
        public string TextToAnalyse { get; set; } = "Analyse this text for me please.";

        public DocumentSentiment? DocumentSentiment = null;

        private readonly string aiServicesKey;
        private readonly string aiServicesRegion;

        public SentimentAnalysisModel(IConfiguration configuration)
        {
            aiServicesKey = configuration["AzureAiServices:Key"] ?? throw new ArgumentNullException("AzureAiServices:Key");
            aiServicesRegion = configuration["AzureAiServices:Region"] ?? throw new ArgumentNullException("AzureAiServices:Region");
        }

        public async Task OnGet()
        {
            await ProcessAnalysis();
        }

        public async Task OnPost()
        {
            await ProcessAnalysis();
        }

        public async Task ProcessAnalysis()
        {
            AzureKeyCredential credential = new(aiServicesKey);
            TextAnalyticsClient client = new(new Uri($"https://{aiServicesRegion}.api.cognitive.microsoft.com/"), credential);

            var response = await client.AnalyzeSentimentAsync(TextToAnalyse);
            DocumentSentiment = response.Value;
        }
    }
}
