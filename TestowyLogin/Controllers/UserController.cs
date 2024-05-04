using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestowyLogin.Models;
using TestowyLogin.Services;

namespace TestowyLogin.Controllers
{
    [Route("api/account")]
    [ApiController] //odpowiada za walidacje modelu i zwracanie bledow walidacji
    public class UserController : Controller
    {

        private readonly IUserService _service;
        public UserController(IUserService service) 
        {
            _service = service;
            
        }

        [HttpGet]
        [Route("getusers")]
        [Authorize]
        public ActionResult<List<User>> GetUsers()
        {
            return _service.GetUsers();
        }


        [HttpPost]
        [Route("register")]
        public ActionResult<User> RegisterUser(RegisterUserDto dto)
        {
            _service.RegisterUser(dto);
            return Json(dto);
        }


        [HttpPost]
        [Route("login")]
        public ActionResult Login([FromBody] LoginDto dto)
        {

            string token = _service.GenerateJwt(dto);
            return Ok(token);
        }

    }
}
