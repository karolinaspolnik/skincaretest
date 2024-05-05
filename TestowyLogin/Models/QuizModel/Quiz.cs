using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TestowyLogin.Models.QuizModel
{
    public class Quiz
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId QuizId { get; set; }
        public List<Question> QuestionIds { get; set; }
    }

    public class Question
    {
        public ObjectId QuestionId { get; set; }
        public string Text { get; set; }
        public List<Answer> Answers { get; set; }
    }

    public class Answer
    {

        public ObjectId AnswerId { get; set; }
        public string Text { get; set; }
        public ObjectId? AssociatedProductId { get; set; }
        public ObjectId NextQuestionId { get; set; }
    }
}
