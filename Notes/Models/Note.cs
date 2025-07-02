using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace Notes.Models
{
    public class Note
    {
        [BsonId] // Pour faire de cette propriété la clé primaire du document
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int patId { get; set; }
        public string patient { get; set; } = "";
        public string content { get; set; } = ""; 
    }
}
