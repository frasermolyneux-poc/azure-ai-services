using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{
    public class ViewImagesModel : PageModel
    {
        public IEnumerable<Image> Images { get; set; }

        private readonly string storageAccountName;

        public ViewImagesModel(IConfiguration configuration)
        {
            storageAccountName = configuration["AppData:StorageAccountName"] ?? throw new ArgumentNullException("AppData:StorageAccountName");
        }

        public async Task OnGetAsync()
        {
            var blobContainer = GetBlobContainer();
            var blobs = blobContainer.GetBlobsAsync();
            var images = new List<Image>();

            await foreach (var blob in blobs)
            {
                var blobClient = blobContainer.GetBlobClient(blob.Name);
                var url = blobClient.Uri.ToString();
                images.Add(new Image { Url = url });
            }

            Images = images;
        }

        private BlobContainerClient GetBlobContainer()
        {
            var credential = new DefaultAzureCredential();
            var blobServiceClient = new BlobServiceClient(new Uri($"https://{storageAccountName}.blob.core.windows.net/"), credential);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient("images");
            return blobContainerClient;
        }
    }

    public class Image
    {
        public string Url { get; set; }
    }
}