using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using UsChat.ASP.Models.Message;
using UsChat.DTO.DTO;

namespace UsChat.ASP.Models.User
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "UserName kan niet langer zijn dan 50 karakters.")]
        public string Username { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "Email kan niet langer zijn dan 250 karakters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "Wachtwoord kan niet langer zijn dan 100 karakters.")]
        public string Password { get; set; }
    }
}
