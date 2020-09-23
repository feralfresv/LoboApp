using System.ComponentModel.DataAnnotations;

namespace WolfApp.Models.Request
{
    public class NewUserRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        public string Name { get; set; }
    }
}