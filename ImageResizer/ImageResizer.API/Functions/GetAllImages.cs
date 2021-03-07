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
    public class GetAllImages
    {
        private readonly ILogger<GetAllImages> _logger;
        private readonly CosmosTableService<ImageEntity> _cosmosTableService;

        public GetAllImages(
            ILogger<GetAllImages> logger,
            CosmosTableService<ImageEntity> cosmosTableService)
        {
            _logger = logger;
            _cosmosTableService = cosmosTableService;
        }

        [FunctionName(nameof(GetAllImages))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Image")] HttpRequest req)
        {
            IActionResult result;

            try
            {
                // TODO: pass in username once authentication is working
                var results = await _cosmosTableService.RetrieveAllImagesForUser("will");

                result = new OkObjectResult(results);
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
