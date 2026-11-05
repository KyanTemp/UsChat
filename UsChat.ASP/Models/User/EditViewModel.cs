using System.ComponentModel.DataAnnotations;

namespace UsChat.ASP.Models.User
{
    public class EditViewModel
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
        [MinLength(8, ErrorMessage = "Wachtwoord moet minstens 8 karakters lang zijn.")]
        public string? Password { get; set; }
        [StringLength(100, ErrorMessage = "Wachtwoord kan niet langer zijn dan 100 karakters.")]
        [MinLength(8, ErrorMessage = "Wachtwoord moet minstens 8 karakters lang zijn.")]
        public string? OldPassword { get; set; }
        [StringLength(100, ErrorMessage = "Wachtwoord kan niet langer zijn dan 100 karakters.")]
        [MinLength(8, ErrorMessage = "Wachtwoord moet minstens 8 karakters lang zijn.")]
        public string? NewPassword { get; set; }
        [StringLength(100, ErrorMessage = "Wachtwoord kan niet langer zijn dan 100 karakters.")]
        [MinLength(8, ErrorMessage = "Wachtwoord moet minstens 8 karakters lang zijn.")]
        public string? VerifyPassword { get; set; }


    }
}
