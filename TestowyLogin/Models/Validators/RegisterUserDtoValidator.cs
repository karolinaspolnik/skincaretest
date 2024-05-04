using FluentValidation;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading;
using TestowyLogin.Database;

namespace TestowyLogin.Models.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        private readonly IMongoCollection<User> _usersCollection;

        public RegisterUserDtoValidator(IOptions<DatabaseSettings> skinCareDatabaseSettings)
        {
            var client = new MongoClient(skinCareDatabaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(skinCareDatabaseSettings.Value.DatabaseName);
            _usersCollection = database.GetCollection<User>(skinCareDatabaseSettings.Value.UsersCollectionName);


            RuleFor(x => x.Email).NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password).MinimumLength(6);
            RuleFor(x => x.ConfirmPassword).Equal(e => e.Password);
            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var existingUser = _usersCollection.Find(u => u.Email == value).FirstOrDefault();
                    if (existingUser != null)
                    {
                        context.AddFailure("Email", "That e-mail is taken.");
                    }
                });

        }




    }
}
