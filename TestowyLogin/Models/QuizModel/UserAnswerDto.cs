using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace TestowyLogin.Models.QuizModel
{
    public class UserAnswerDto
    {
        public string QuizId { get; set; }
        public string QuestionId { get; set; }
        public string AnswerId { get; set; }

    }
}
