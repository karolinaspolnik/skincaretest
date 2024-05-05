using MongoDB.Bson;
using System.Net.Mime;
using TestowyLogin.Models;
using TestowyLogin.Models.QuizModel;
using static System.Net.Mime.MediaTypeNames;

namespace TestowyLogin.QuizStructure
{
    public class NewRecords
    {
        public static List<Product> GetProducts()
        {
            var products = new List<Product>
            {
                new Product
                {
                    ProductName = "Name",
                    ProductCategory = "Category",
                    ProductDescription = "Desc"
                }
            };
            return products;
        }




        public static List<Quiz> GetQuizzes()
        {
            var quizzes = new List<Quiz>
            {
                new Quiz
                {
                    QuestionIds = new List<Question>
                    {
                        
                        new Question
                        {
                            QuestionId = Constants.Question1Id,
                            Text = "Question 1 ",
                            Answers = new List<Answer>
                            {
                                new Answer {AnswerId = ObjectId.GenerateNewId(), Text = "Answer to question 1 leading to question 3", NextQuestionId = Constants.Question3Id },
                                new Answer {AnswerId = ObjectId.GenerateNewId(), Text = "Answer to question 1 leading to question 4", NextQuestionId = Constants.Question4Id }
                            }
                        },
                        new Question
                        {
                            QuestionId = Constants.Question2Id,
                            Text = "Question 2 ",
                            Answers = new List<Answer>
                            {
                                new Answer {AnswerId = ObjectId.GenerateNewId(), Text = "Answer to question 2 leading to question 3", NextQuestionId = Constants.Question3Id },
                                new Answer {AnswerId = ObjectId.GenerateNewId(), Text = "Answer to question 2 leading to question 4", NextQuestionId = Constants.Question4Id  }
                            }
                        },
                        new Question
                        {
                            QuestionId = Constants.Question3Id,
                            Text = "Question 3 ",
                            Answers = new List<Answer>
                            {
                                new Answer {AnswerId = ObjectId.GenerateNewId(), Text = "Answer to question 3 leading to question 4", NextQuestionId = Constants.Question4Id },
                                new Answer {AnswerId = ObjectId.GenerateNewId(), Text = "Answer to question 3 leading to question 2", NextQuestionId = Constants.Question2Id  }
                            }
                        },
                        new Question
                        {
                            QuestionId = Constants.Question4Id,
                            Text = "Question 4 ",
                            Answers = new List<Answer>
                            {
                                new Answer {AnswerId = ObjectId.GenerateNewId(), Text = "Answer to question 4 leading to question 1", NextQuestionId = Constants.Question1Id },
                                new Answer {AnswerId = ObjectId.GenerateNewId(), Text = "Answer to question 4 leading to question 2", NextQuestionId = Constants.Question2Id  }
                            }
                        },

                    }

                }
            };
            return quizzes;

        }
    }
}
