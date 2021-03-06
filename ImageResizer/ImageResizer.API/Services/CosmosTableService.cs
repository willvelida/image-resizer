using ImageResizer.API.Models;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageResizer.API.Services
{
    public class CosmosTableService<T> where T : TableEntity, new()
    {
        private volatile CloudTable _cloudTable;
        private readonly object _cloudTableLock = new object();

        private readonly string _storageConnectionString;
        private readonly string _tableName;
        private readonly TableClientConfiguration _tableClientConfiguration;

        public CosmosTableService(
            string storageConnectionString,
            string tableName,
            TableClientConfiguration tableClientConfiguration = null)
        {
            _storageConnectionString = storageConnectionString;
            _tableName = tableName;
            _tableClientConfiguration = tableClientConfiguration == null ? new TableClientConfiguration() : tableClientConfiguration;
        }

        public virtual async Task<T> InsertOrMerge(T tableEntity)
        {
            var insertOrReplaceOperation = TableOperation.InsertOrMerge(tableEntity);
            var result = await CloudTable.ExecuteAsync(insertOrReplaceOperation);
            var returnedValue = result.Result as T;
            return returnedValue;
        }

        public virtual async Task<T> RetrieveEntity(string username, string id)
        {
            var retrieveOperation = TableOperation.Retrieve<T>(username, id);
            TableResult result = await CloudTable.ExecuteAsync(retrieveOperation);
            var returnedValue = result.Result as T;
            return returnedValue;
        }

        public virtual async Task DeleteEntity(T entityToDelete)
        {
            var deleteOperation = TableOperation.Delete(entityToDelete);
            TableResult result = await CloudTable.ExecuteAsync(deleteOperation);
        }

        public virtual async Task<List<ImageDTO>> RetrieveAllImagesForUser(string username)
        {
            List<ImageDTO> imageDTOs = new List<ImageDTO>();

            TableQuery<ImageEntity> imageScanQuery = new TableQuery<ImageEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, username));

            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<ImageEntity> segment = await CloudTable.ExecuteQuerySegmentedAsync(imageScanQuery, token);
                token = segment.ContinuationToken;
                foreach (ImageEntity entity in segment)
                {
                    ImageDTO imageDTO = new ImageDTO { Id = entity.RowKey, Username = entity.PartitionKey, ImageUrl = entity.ImageUrl };
                    imageDTOs.Add(imageDTO);
                }
            } while (token != null);

            return imageDTOs;
        }

        private CloudTable CloudTable
        {
            get
            {
                if (_cloudTable == null)
                {
                    lock (_cloudTableLock)
                    {
                        if (_cloudTable == null)
                        {
                            var storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
                            var cloudTableClient = storageAccount.CreateCloudTableClient(_tableClientConfiguration);
                            _cloudTable = cloudTableClient.GetTableReference(_tableName);

                            if (_cloudTable.Exists() == false)
                                throw new ArgumentException($"Table {_tableName} does not exist in storage account");
                        }
                    }
                }
                return _cloudTable;
            }
        }
    }
}
