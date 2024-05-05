using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TestowyLogin.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductCategory { get; set; }
    }
}
