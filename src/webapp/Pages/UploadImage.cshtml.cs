using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{
    public class UploadImageModel : PageModel
    {
        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        private readonly string storageAccountName;

        public UploadImageModel(IConfiguration configuration)
        {
            storageAccountName = configuration["AppData:StorageAccountName"] ?? throw new ArgumentNullException("AppData:StorageAccountName");
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var blobContainer = GetBlobContainer();
            var blobName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
            var blobClient = blobContainer.GetBlobClient(blobName);

            using (var stream = ImageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream);
            }

            return RedirectToPage();
        }

        private BlobContainerClient GetBlobContainer()
        {
            var credential = new DefaultAzureCredential();
            var blobServiceClient = new BlobServiceClient(new Uri($"https://{storageAccountName}.blob.core.windows.net/"), credential);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient("images");
            return blobContainerClient;
        }
    }
}
