using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TestowyLogin.Models
{
    public class RegisterUserDto
    {

        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Passwords do not match!")] //jakis komunikat lepszy?
        public string ConfirmPassword { get; set; }

    }
}
