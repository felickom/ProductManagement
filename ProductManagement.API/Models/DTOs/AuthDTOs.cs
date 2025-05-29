using System.ComponentModel.DataAnnotations;

namespace ProductManagement.API.Models.DTOs
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }


    public class AuthResponse
    {
        public string Token { get; set; }
        public string Username { get; set; }
    }
} 