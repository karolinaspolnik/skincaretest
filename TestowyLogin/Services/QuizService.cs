using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using NLog.Web.LayoutRenderers;
using TestowyLogin.Database;
using TestowyLogin.Exceptions;
using TestowyLogin.Models;
using TestowyLogin.Models.QuizModel;

namespace TestowyLogin.Services
{
    public interface IQuizService
    {
        DisplayQuestionDto GetFirstQuestion();
        DisplayQuestionDto GetQuestion(string quizId);
        void SaveUserAnswer(UserAnswerDto userAnswer);
    }
    public class QuizService : IQuizService
    {
        private readonly IMongoCollection<Quiz> _quizzes;
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<UserAnswer> _useranswers;
        private readonly ILogger<UserService> _logger;

        public QuizService(IOptions<DatabaseSettings> skinCareDatabaseSettings, IConfiguration configuration, ILogger<UserService> logger)
        {
            _logger = logger;

            var client = new MongoClient(skinCareDatabaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(skinCareDatabaseSettings.Value.DatabaseName);
            _quizzes = database.GetCollection<Quiz>(skinCareDatabaseSettings.Value.QuizCollectionName);
            _products = database.GetCollection<Product>(skinCareDatabaseSettings.Value.ProductsCollectionName);
            _useranswers = database.GetCollection<UserAnswer>(skinCareDatabaseSettings.Value.UserAnswerCollectionName);
        }

        public DisplayQuestionDto GetFirstQuestion()
        {
            try
            {
                var firstQuiz = _quizzes.Find(q => true).FirstOrDefault();

                if (firstQuiz != null && firstQuiz.QuestionIds.Count > 0)
                {
                    var firstQuestion = firstQuiz.QuestionIds[0];
                    var questionDto = new DisplayQuestionDto()
                    {
                        QuizId = firstQuiz.QuizId.ToString(),
                        QuestionId = firstQuestion.QuestionId.ToString(),
                        Text = firstQuestion.Text,
                        Answers = firstQuestion.Answers.Select(a => new DisplayAnswerDto {AnswerId = a.AnswerId.ToString(), Text = a.Text }).ToList()
                    };
                    return questionDto;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Wystapil blad"); //do poprawienia
                return null;
            }
        }

        public DisplayQuestionDto GetQuestion(string quizId)
        {
            // Pobierz ostatnią udzieloną odpowiedź dla danego quizu
            var lastUserAnswer = _useranswers
                .Find(ua => ua.QuizId.ToString() == quizId)
                .SortByDescending(ua => ua.CreatedTime)
                .FirstOrDefault();

            // Jeśli istnieje ostatnia odpowiedź i ma NextQuestionId, pobierz kolejne pytanie
            if (lastUserAnswer != null && lastUserAnswer.NextQuestionId != null)
            {
                var quizE = _quizzes.Find(q => q.QuizId.ToString() == quizId).FirstOrDefault();
                if (quizE != null)
                {
                    var nextQuestion = quizE.QuestionIds.FirstOrDefault(q => q.QuestionId == lastUserAnswer.NextQuestionId);
                    if (nextQuestion != null)
                    {
                        return MapToDisplayQuestionDto(nextQuestion);
                    }
                }
            }

            // Jeśli nie ma ostatniej odpowiedzi lub nie można znaleźć kolejnego pytania, pobierz pierwsze pytanie z quizu
            var quiz = _quizzes.Find(q => q.QuizId.ToString() == quizId).FirstOrDefault();
            if (quiz != null && quiz.QuestionIds.Count > 0)
            {
                var firstQuestion = quiz.QuestionIds.FirstOrDefault();
                if (firstQuestion != null)
                {
                    return MapToDisplayQuestionDto(firstQuestion);
                }
            }

            // Jeśli żadne pytanie nie zostało znalezione, zwróć null lub inne odpowiednie wartości
            return null;
        }




        private DisplayQuestionDto MapToDisplayQuestionDto(Question question)
        {
            var displayQuestionDto = new DisplayQuestionDto
            {
                QuestionId = question.QuestionId.ToString(),
                Text = question.Text,
                Answers = question.Answers.Select(a => new DisplayAnswerDto
                {
                    AnswerId = a.AnswerId.ToString(),
                    Text = a.Text
                }).ToList()
            };

            return displayQuestionDto;
        }

        public void SaveUserAnswer([FromBody]UserAnswerDto dto)
        {
            ObjectId questionId = ObjectId.Parse(dto.QuestionId);
            ObjectId answerId = ObjectId.Parse(dto.AnswerId);
            ObjectId quizId = ObjectId.Parse(dto.QuizId);
            

            var quiz = _quizzes.Find(q => q.QuestionIds.Any(question => question.QuestionId == questionId)).FirstOrDefault();
            var question = quiz.QuestionIds.FirstOrDefault(q => q.QuestionId == questionId);
            var answer = question.Answers.FirstOrDefault(a => a.AnswerId == answerId);
        

            var userAnswer = new UserAnswer()
            {
                QuizId = quizId,
                QuestionId = questionId,
                AnswerId = answerId,
                NextQuestionId = answer.NextQuestionId,
                CreatedTime = DateTime.Now

            };
            _useranswers.InsertOne(userAnswer);
        }


        
    }


}
