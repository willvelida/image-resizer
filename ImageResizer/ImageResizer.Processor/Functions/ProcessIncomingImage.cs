using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ImageResizer.Processor.Models;
using ImageResizer.Processor.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ImageResizer.Processor.Functions
{
    public class ProcessIncomingImage
    {
        private readonly ILogger<ProcessIncomingImage> _logger;
        private readonly IAzureBlobService _azureBlobService;
        private readonly IConfiguration _configuration;

        public ProcessIncomingImage(
            ILogger<ProcessIncomingImage> logger,
            IAzureBlobService azureBlobService,
            IConfiguration configuration)
        {
            _logger = logger;
            _azureBlobService = azureBlobService;
            _configuration = configuration;
        }

        [FunctionName(nameof(ProcessIncomingImage))]
        public async Task Run([QueueTrigger("myqueue-items", Connection = "")]string myQueueItem)
        {
            try
            {
                // Deserialize Queue message body into Image object
                var image = JsonConvert.DeserializeObject<Image>(myQueueItem);
                byte[] imageData;

                // Use Image object url to download content
                using (var wc= new WebClient())
                {
                    imageData = wc.DownloadData(image.ImageUrl);
                }

                // Add a guid to name of the blob
                var ext = Path.GetExtension(image.ImageUrl);
                var fileName = Path.GetFileNameWithoutExtension(image.ImageUrl);
                var blobName = fileName + "." + Guid.NewGuid().ToString() + "." + ext;
                var blobClient = _azureBlobService.GetBlobClient(_configuration[""], _configuration[""], blobName);

                // Save the new blob into Azure Blob Storage
                await blobClient.UploadAsync(blobName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown: {ex.Message}");
                throw;
            }
        }
    }
}
