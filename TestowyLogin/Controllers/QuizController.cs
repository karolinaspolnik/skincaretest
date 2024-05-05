using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TestowyLogin.Models.QuizModel;
using TestowyLogin.Models.Validators;
using TestowyLogin.Services;

namespace TestowyLogin.Controllers
{
    [Route("api/quiz")]
    [ApiController]
    public class QuizController : Controller
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        //[HttpGet]
        //[Route("firstquestion")]
        //public ActionResult<DisplayQuestionDto> GetFirstQuestion() //wyswietlanie pytania, jezeli nie ma okreslonego to wysylamy piewsze
        //{
        //    var firstQuestion = _quizService.GetFirstQuestion();

        //    if (firstQuestion != null)
        //    {
        //        return Ok(firstQuestion);
        //    }
        //    else
        //    {
        //        return NotFound("Brak dostępnych pytań w quizu.");
        //    }

        //}

        [HttpPost]
        [Route("answer")]
        public ActionResult<UserAnswer> SaveAnswer([FromBody]UserAnswerDto dto)
        {
            _quizService.SaveUserAnswer(dto);
            return Json(dto);
        }

        [HttpGet]
        [Route("question/{quizId}")]
        public ActionResult<DisplayQuestionDto> GetQuestion(string quizId) 
        {
            var firstQuestion = _quizService.GetQuestion(quizId);

            if (firstQuestion != null)
            {
                return Ok(firstQuestion);
            }
            else
            {
                return NotFound("Brak dostępnych pytań w quizu.");
            }

        }





    }
}
