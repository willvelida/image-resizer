using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ImageResizer.API.Services;
using ImageResizer.API.Models;

namespace ImageResizer.API.Functions
{
    public class CreateImage
    {
        private readonly ILogger<CreateImage> _logger;
        private readonly CosmosTableService<ImageEntity> _cosmosTableService;

        public CreateImage(
            ILogger<CreateImage> logger,
            CosmosTableService<ImageEntity> cosmosTableService)
        {
            _logger = logger;
            _cosmosTableService = cosmosTableService;
        }

        [FunctionName(nameof(CreateImage))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Resize")] HttpRequest req)
        {
            IActionResult result;

            try
            {
                string input = await new StreamReader(req.Body).ReadToEndAsync();

                var imageRequestObject = JsonConvert.DeserializeObject<ImageDTO>(input);

                var imageEntity = new ImageEntity(imageRequestObject.Username, imageRequestObject.ImageUrl);


                if (imageEntity.PartitionKey == null || imageEntity.RowKey == null)
                {
                    _logger.LogError("The image create request was in a incorrect format");
                    result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                }
                else
                {
                    await _cosmosTableService.InsertOrMerge(imageEntity);

                    result = new StatusCodeResult(StatusCodes.Status201Created);
                }              
            }
            catch (Exception ex)
            {
                // TODO Send Exception event to external service
                _logger.LogError($"Internal Server Error: Exception thrown: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
