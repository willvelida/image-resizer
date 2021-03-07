using ImageResizer.API.Models;
using ImageResizer.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ImageResizer.API.Functions
{
    public class DeleteImage
    {
        private readonly ILogger<DeleteImage> _logger;
        private readonly CosmosTableService<ImageEntity> _cosmosTableService;

        public DeleteImage(
            ILogger<DeleteImage> logger,
            CosmosTableService<ImageEntity> cosmosTableService)
        {
            _logger = logger;
            _cosmosTableService = cosmosTableService;
        }

        [FunctionName(nameof(DeleteImage))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Image/{id}")] HttpRequest req,
            string id)
        {
            IActionResult result;

            try
            {
                var imageEntity = await _cosmosTableService.RetrieveEntity("will", id);

                if (imageEntity == null)
                {
                    result = new StatusCodeResult(StatusCodes.Status404NotFound);
                }
                else
                {
                    await _cosmosTableService.DeleteEntity(imageEntity);
                    _logger.LogInformation($"Image with ID:{imageEntity.RowKey} successfully deleted");
                    result = new StatusCodeResult(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error. Exception thrown: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
