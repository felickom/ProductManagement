using Microsoft.AspNetCore.Mvc;
using ProductManagement.API.Models.DTOs;
using ProductManagement.API.Services;
using ProductManagement.API.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using ProductManagement.API.Models;
using System.Net;

namespace ProductManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ProductmanagementContext _context;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ProductmanagementContext context, JwtService jwtService, ILogger<AuthController> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginRequest request)
        {
            _logger.LogInformation("Login attempt for user: {Username}", request.Username);

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning("Login failed: Username or password is empty");
                return BadRequest(ApiResponse<AuthResponse>.BadRequestResponse("Username and password are required"));
            }

            try
            {
                var user = await _context.Apiusers
                    .FirstOrDefaultAsync(u => u.Username == request.Username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    _logger.LogWarning("Login failed: Invalid credentials for user {Username}", request.Username);
                    return Unauthorized(ApiResponse<AuthResponse>.UnauthorizedResponse("Invalid username or password"));
                }

                var token = _jwtService.GenerateToken(user);

                var response = new AuthResponse
                {
                    Token = token,
                    Username = user.Username
                };

                _logger.LogInformation("Login successful for user: {Username}", request.Username);
                return Ok(ApiResponse<AuthResponse>.SuccessResponse(response, "Login successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Username}", request.Username);
                return StatusCode((int)HttpStatusCode.InternalServerError, 
                    ApiResponse<AuthResponse>.ServerErrorResponse("An error occurred during login"));
            }
        }
    }
} 