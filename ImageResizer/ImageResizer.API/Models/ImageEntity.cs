using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageResizer.API.Models
{
    public class ImageEntity : TableEntity
    {
        public ImageEntity()
        {

        }

        public ImageEntity(string username, string pictureUrl)
        {
            PartitionKey = username;
            RowKey = pictureUrl;
        }
    }
}
