using System.ComponentModel.DataAnnotations;

namespace ProductManagement.API.Models.DTOs
{
    public class LoginRequest
    {
         [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }


    public class AuthResponse
    {
        public string Token { get; set; }
        public string Username { get; set; }
    }
} 