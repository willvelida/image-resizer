using Microsoft.Azure.Cosmos.Table;

namespace ImageResizer.API.Models
{
    public class ImageEntity : TableEntity
    {
        public ImageEntity()
        {

        }

        public ImageEntity(string username, string id)
        {
            PartitionKey = username;
            RowKey = id;
        }

        public string ImageUrl { get; set; }
    }
}
