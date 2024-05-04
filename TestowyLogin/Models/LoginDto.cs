using System.ComponentModel.DataAnnotations;

namespace TestowyLogin.Models
{
    public class LoginDto //data transfer object - do komunikacji z klientem. moze byc uzywany do pobierania informacji od klienta lub do udostepniania tylko wybranych informacji
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
