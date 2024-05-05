using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using TestowyLogin.Database;
using TestowyLogin.Exceptions;
using TestowyLogin.Models;


namespace TestowyLogin.Services
{
    public interface IUserService
    {
        List<User> GetUsers();

        void RegisterUser(RegisterUserDto dto);

        string GenerateJwt(LoginDto dto);
    }

    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<UserService> _logger;
        private readonly AuthenticationSettings _authenticationSettings;

        public UserService(IOptions<DatabaseSettings> skinCareDatabaseSettings, IConfiguration configuration, IPasswordHasher<User> passwordHasher, ILogger<UserService> logger, AuthenticationSettings authenticationSettings)
        {
            _passwordHasher = passwordHasher;
            _logger = logger;
            _authenticationSettings = authenticationSettings;

            var client = new MongoClient(skinCareDatabaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(skinCareDatabaseSettings.Value.DatabaseName);
            _users = database.GetCollection<User>(skinCareDatabaseSettings.Value.UsersCollectionName);
        }

        public List<User> GetUsers() => _users.Find(user => true).ToList();

        public void RegisterUser([FromBody]RegisterUserDto dto) //data transfer object
        {
            _logger.LogError($"User with email: {dto.Email} CREATE action invoked");
            var newUser = new User()
            {
                Email = dto.Email,
                Name = dto.Name
            };
            var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
            newUser.PasswordHash = hashedPassword;
            _users.InsertOne(newUser);
        }


        public string GenerateJwt(LoginDto dto)
        {
            var user = this._users.Find(user => user.Email == dto.Email).FirstOrDefault();
            if (user == null)
            {
                throw new BadRequestException("Invalid username or password");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password); //user was null
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid username or password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, $"{user.Name}")
                //dodac claimy jakies - czy id musze miec? id zostalo dodane bo sie wypierdalalo lekcja 40
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

    }
}
