using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TestowyLogin.Database;
using TestowyLogin.Models;
using TestowyLogin.Models.QuizModel;
using TestowyLogin.QuizStructure;

namespace TestowyLogin.Services
{
    public interface IDataInitializerService
    {
        void InitializeData();
        void InitializeQuiz(List<Quiz> quizzes);
        void InitializeProducts(List<Product> products);
    }

    public class DataInitializerService : IDataInitializerService
    {
        private readonly IMongoCollection<Quiz> _quizzes;
        private readonly IMongoCollection<Product> _products;
        private readonly bool _dataInitialized;

        public DataInitializerService(IOptions<DatabaseSettings> skinCareDatabaseSettings)
        {
            var client = new MongoClient(skinCareDatabaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(skinCareDatabaseSettings.Value.DatabaseName);
            _quizzes = database.GetCollection<Quiz>(skinCareDatabaseSettings.Value.QuizCollectionName);
            _products = database.GetCollection<Product>(skinCareDatabaseSettings.Value.ProductsCollectionName);
        }

        public void InitializeData()
        {
            if(!CheckIfDataInitialized())
            {
                var products = NewRecords.GetProducts();
                var quizzes = NewRecords.GetQuizzes();

                InitializeProducts(products);
                InitializeQuiz(quizzes);

                SetDataInitializedFlagAsync();

                Console.WriteLine("Dane zostały pomyślnie zainicjalizowane.");
            }
            else
            {
                Console.WriteLine("Dane już zostały zainicjalizowane:");
            }
        }

        private bool CheckIfDataInitialized()
        {
            var productsCount = _products.CountDocuments(FilterDefinition<Product>.Empty);
            var quizzesCount = _quizzes.CountDocuments(FilterDefinition<Quiz>.Empty);
            return productsCount > 0 && quizzesCount > 0;
        }
        private bool _isDataInitialized = false;
        public async Task<bool> IsDataInitializedAsync()
        {
            return _isDataInitialized;
        }

        public async Task SetDataInitializedFlagAsync()
        {
            _isDataInitialized = true;
        }

        public void InitializeQuiz(List<Quiz> quizzes)
        {
            _quizzes.InsertMany(quizzes);
        }

        public void InitializeProducts(List<Product> products)
        {
            _products.InsertMany(products);
        }


    }

  
}
