namespace TestowyLogin.Models.QuizModel
{
    public class DisplayQuestionDto
    {
        public string QuizId { get; set; }
        public string QuestionId { get; set; }
        public string Text { get; set; }
        public List<DisplayAnswerDto> Answers { get; set; }
    }
}
