using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TestowyLogin.Models.QuizModel
{
    public class UserAnswer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public ObjectId QuizId { get; set; }
        public ObjectId QuestionId { get; set; }
        public ObjectId AnswerId { get; set; }
        public DateTime CreatedTime { get; set; }
        public ObjectId NextQuestionId { get; set; }

    }
}
