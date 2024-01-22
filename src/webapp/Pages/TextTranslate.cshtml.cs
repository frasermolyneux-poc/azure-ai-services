using System.ComponentModel.DataAnnotations;
using Azure;
using Azure.AI.Translation.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace MyApp.Namespace
{
    public class TextTranslateModel : PageModel
    {
        private readonly IMemoryCache memoryCache;

        [BindProperty]
        [Required(ErrorMessage = "Please enter some text to translate.")]
        [MinLength(1, ErrorMessage = "Please enter some text to translate.")]
        public string TextToTranslate { get; set; } = "Translate this text for me please.";

        [BindProperty]
        public string TargetLanguage { get; set; } = "fr";

        public List<string> TargetLanguageOptions { get; set; } = new List<string>();

        public TranslatedTextItem? TranslatedText = null;

        private readonly string aiServicesKey;
        private readonly string aiServicesRegion;

        public TextTranslateModel(IConfiguration configuration, IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            aiServicesKey = configuration["AzureAiServices:Key"] ?? throw new ArgumentNullException("AzureAiServices:Key");
            aiServicesRegion = configuration["AzureAiServices:Region"] ?? throw new ArgumentNullException("AzureAiServices:Region");
        }

        public async Task OnGet()
        {
            await PopulateLanguages();
            await ProcessTranslation();
        }

        public async Task OnPost()
        {
            await PopulateLanguages();
            await ProcessTranslation();
        }

        public async Task ProcessTranslation()
        {
            AzureKeyCredential credential = new(aiServicesKey);
            TextTranslationClient client = new(credential, aiServicesRegion);

            Response<IReadOnlyList<TranslatedTextItem>> response = await client.TranslateAsync(TargetLanguage, TextToTranslate);
            IReadOnlyList<TranslatedTextItem> translations = response.Value;

            TranslatedText = translations.FirstOrDefault();
        }

        public async Task PopulateLanguages()
        {
            if (memoryCache.TryGetValue("TargetLanguageOptions", out List<string> targetLanguageOptions))
            {
                TargetLanguageOptions = targetLanguageOptions;
            }
            else
            {
                await CacheLanguages();
            }
        }

        public async Task CacheLanguages()
        {
            AzureKeyCredential credential = new(aiServicesKey);
            TextTranslationClient client = new(credential, aiServicesRegion);

            var languages = await client.GetLanguagesAsync();
            var languagesToCache = languages.Value.Translation.Select(x => x.Key).ToList();

            memoryCache.Set("TargetLanguageOptions", languagesToCache);

            TargetLanguageOptions = languagesToCache;
        }
    }
}
