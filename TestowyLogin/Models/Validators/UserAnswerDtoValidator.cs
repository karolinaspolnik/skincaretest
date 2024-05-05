using FluentValidation;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using TestowyLogin.Database;
using TestowyLogin.Models.QuizModel;

namespace TestowyLogin.Models.Validators
{
    public class UserAnswerDtoValidator : AbstractValidator<UserAnswerDto>
    {
        private readonly IMongoCollection<Quiz> _quizzes;

        public UserAnswerDtoValidator(IOptions<DatabaseSettings> skinCareDatabaseSettings)
        {
            var client = new MongoClient(skinCareDatabaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(skinCareDatabaseSettings.Value.DatabaseName);
            _quizzes = database.GetCollection<Quiz>(skinCareDatabaseSettings.Value.QuizCollectionName);

            RuleFor(x => x.QuizId)
                .NotEmpty()
                .Length(24);
            RuleFor(x=>x.AnswerId)
                .NotEmpty()
                .Length(24);
            RuleFor(x => x.QuestionId)
                .NotEmpty()
                .Length(24);

            RuleFor(x => x.QuizId)
                .Custom((value, context) =>
                {
                    var existingQuiz = _quizzes.Find(q => q.QuizId.ToString() == value).FirstOrDefault();
                    if (existingQuiz == null)
                    {
                        context.AddFailure("QuizId", "Quiz doesn't exist");
                    }
                });
            RuleFor(x => x.QuestionId)
                .Custom((value, context) =>
                {
                    var existingQuestion = _quizzes.Find(q => q.QuestionIds.Any(question => question.QuestionId.ToString() == value)).FirstOrDefault();
                    if (existingQuestion == null)
                    {
                        context.AddFailure("QuestionId", "Question doesn't exist");
                    }
                });
            RuleFor(x => x.AnswerId)
                .Custom((value, context) =>
                {
                    var existingAnswer = _quizzes.Find(q => q.QuestionIds.Any(question => question.Answers.Any(answer => answer.AnswerId.ToString() == value))).FirstOrDefault();
                    if (existingAnswer == null)
                    {
                        context.AddFailure("AnswerId", "Answer doesn't exist");
                    }
                });
        }


    }
}
