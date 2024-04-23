using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TestowyLogin.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("Email")]
        public string Email { get; set; }
        public string Name { get; set; }

        [BsonElement("Password")]
        public string PasswordHash { get; set; }

    }
}
