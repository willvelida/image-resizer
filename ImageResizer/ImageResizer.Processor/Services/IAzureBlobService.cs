using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageResizer.Processor.Services
{
    public interface IAzureBlobService
    {
        BlobClient GetBlobClient(string connectionString, string blobContainerName, string blobName);
    }
}
