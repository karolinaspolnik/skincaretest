using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using TestowyLogin.Models;


namespace TestowyLogin.Services
{
    public interface IUserService
    {
        string GenerateJwt(LoginDto dto);
    }
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> users;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;

        public UserService(IConfiguration configuration, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
            var client = new MongoClient(configuration.GetConnectionString("skincaremongodb"));
            var database = client.GetDatabase("skincaremongodb");
            users = database.GetCollection<User>("Users"); //czy w kazdym service powinno byc odniesienie do bazy
        }


        public List<User> GetUsers() => users.Find(user => true).ToList();

        public void RegisterUser(RegisterUserDto dto) //data transfer object
        {
            var newUser = new User()
            {
                Email = dto.Email,
                Name = dto.Name
            };
            var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
            newUser.PasswordHash = hashedPassword;
            users.InsertOne(newUser);
        }


        public string GenerateJwt(LoginDto dto)
        {
            var user = this.users.Find(user => user.Email == dto.Email).FirstOrDefault();
            if (user == null)
            {
                //obsluga wyjatkow przez middleware
                //weryfikacja emaila (juz jest)
                //weryfikacja hasla (jest)
                //
                Console.WriteLine("invalid username or password"); //obsluzyc wyjatek
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password); //user was null
            if (result == PasswordVerificationResult.Failed)
            {
                Console.WriteLine("invalid username or password"); //obsluzyc wyjatek
                return null;
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, $"{user.Name}")
                //dodac claimy jakies - czy id musze miec? id zostalo dodane bo sie wypierdalalo
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
