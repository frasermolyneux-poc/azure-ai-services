using Azure;
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{
    public class ImageAnalysisModel : PageModel
    {
        [BindProperty]
        public string ImageUrl { get; set; }

        public ImageAnalysisResult? AnalysisResult = null;

        private readonly string aiServicesKey;
        private readonly string aiServicesRegion;

        public ImageAnalysisModel(IConfiguration configuration)
        {
            aiServicesKey = configuration["AzureAiServices:Key"] ?? throw new ArgumentNullException("AzureAiServices:Key");
            aiServicesRegion = configuration["AzureAiServices:Region"] ?? throw new ArgumentNullException("AzureAiServices:Region");
        }

        public void OnGet()
        {
        }

        public async Task OnPost()
        {
            AzureKeyCredential credential = new(aiServicesKey);
            var serviceOptions = new VisionServiceOptions(new Uri($"https://{aiServicesRegion}.api.cognitive.microsoft.com/"), credential);

            using var imageSource = VisionSource.FromUrl(new Uri(ImageUrl));

            var analysisOptions = new ImageAnalysisOptions()
            {
                Features =
                      ImageAnalysisFeature.CropSuggestions
                    //| ImageAnalysisFeature.Caption
                    //| ImageAnalysisFeature.DenseCaptions
                    | ImageAnalysisFeature.Objects
                    | ImageAnalysisFeature.People
                    | ImageAnalysisFeature.Text
                    | ImageAnalysisFeature.Tags,
            };

            using var analyzer = new ImageAnalyzer(serviceOptions, imageSource, analysisOptions);

            AnalysisResult = await analyzer.AnalyzeAsync();
        }
    }
}
