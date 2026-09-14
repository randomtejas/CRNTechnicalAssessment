using Microsoft.AspNetCore.Mvc;
using SDTechnicalAssessment.Application.DTOs;
using SDTechnicalAssessment.Application.Interfaces;

namespace SDTechnicalAssessment.Controllers
{
    // ------------------------------------------------------------
    // This controller handles authentication-related API requests.
    // ------------------------------------------------------------
    [ApiController]

    // ------------------------------------------------------------
    // Base route:
    //
    // /api/auth
    // ------------------------------------------------------------
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // --------------------------------------------------------
        // Constructor.
        //
        // IAuthService is injected through Dependency Injection.
        // --------------------------------------------------------
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // --------------------------------------------------------
        // POST: api/auth/login
        //
        // This endpoint receives username and password.
        // If credentials are correct, it returns a JWT token.
        // --------------------------------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // ----------------------------------------------------
            // Send the login information to AuthService.
            // ----------------------------------------------------
            var response = await _authService.LoginAsync(dto);

            // ----------------------------------------------------
            // If response is null, username/password is incorrect.
            // ----------------------------------------------------
            if (response == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password."
                });
            }

            // ----------------------------------------------------
            // Login successful.
            //
            // Return the JWT access token to the client.
            // ----------------------------------------------------
            return Ok(response);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
        {
            // Send the refresh token to the authentication service.
            var response = await _authService.RefreshTokenAsync(dto);

            // If the refresh token is invalid, expired, or revoked,
            // return 401 Unauthorized.
            if (response == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid, expired, or revoked refresh token."
                });
            }

            // Return the newly generated access token
            // and refresh token.
            return Ok(response);
        }
    }
}