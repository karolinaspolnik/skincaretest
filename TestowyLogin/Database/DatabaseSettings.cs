namespace TestowyLogin.Database
{
    public interface IDatabaseSettings
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
        string UsersCollectionName { get; set; }
        string QuizCollectionName { get; set; }
        string ProductsCollectionName { get; set; }
        string UserAnswerCollectionName { get; set; }

    }

    public class DatabaseSettings : IDatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public string UsersCollectionName { get; set; }
        public string QuizCollectionName { get; set; }
        public string ProductsCollectionName { get; set; }
        public string UserAnswerCollectionName { get; set; }
    }
}
