using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageResizer.Processor.Services
{
    public class AzureBlobService : IAzureBlobService
    {
        public BlobClient GetBlobClient(string connectionString, string blobContainerName, string blobName)
        {
            return new BlobClient(connectionString, blobContainerName, blobName);
        }
    }
}
