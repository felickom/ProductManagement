using Microsoft.AspNetCore.Mvc;
using ProductManagement.API.Models.DTOs;
using ProductManagement.API.Services;
using ProductManagement.API.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace ProductManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ProductmanagementContext _context;
        private readonly JwtService _jwtService;

        public AuthController(ProductmanagementContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var user = await _context.Apiusers
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Username = user.Username
            };
        }
    }
} 