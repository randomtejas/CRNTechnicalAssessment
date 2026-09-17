using Microsoft.IdentityModel.Tokens;
using SDTechnicalAssessment.Application.DTOs;
using SDTechnicalAssessment.Application.Interfaces;
using SDTechnicalAssessment.Domain.Entities;
using SDTechnicalAssessment.Infrastructure.Data.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using SDTechnicalAssessment.Infrastructure.Security;

namespace SDTechnicalAssessment.Application.Services
{
    // ------------------------------------------------------------
    // This service contains the business logic for authentication.
    //
    // Responsibilities:
    // 1. Find the user from the database.
    // 2. Validate username and password.
    // 3. Generate a JWT access token.
    // ------------------------------------------------------------
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
     

        // Repository used to store and retrieve refresh tokens.
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        private readonly IConfiguration _configuration;


        private readonly PasswordHasher<User> _passwordHasher;


        private readonly RefreshTokenHasher _refreshTokenHasher;

        // --------------------------------------------------------
        // Constructor.
        //
        // IUserRepository is used to find users.
        // IConfiguration is used to read JWT settings from
        // appsettings.json.
        // --------------------------------------------------------
        public AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IConfiguration configuration)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;

            // PasswordHasher securely hashes and verifies user passwords.
            _passwordHasher = new PasswordHasher<User>();
            _refreshTokenHasher = new RefreshTokenHasher();
        }

        // --------------------------------------------------------
        // Login method.
        // --------------------------------------------------------
        public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
        {
            // ----------------------------------------------------
            // Find the user using the username entered during login.
            // ----------------------------------------------------
            var user = await _userRepository
                .GetByUsernameAsync(dto.Username);

            // ----------------------------------------------------
            // If user does not exist, login fails.
            // ----------------------------------------------------
            if (user == null)
            {
                return null;
            }

            // ----------------------------------------------------
            // Check whether the supplied password matches
            // the password stored for the user.
            //
            // NOTE:
            // Verify the entered password against the hashed password
            // stored for the user in the database.
            // ----------------------------------------------------
            // Verify the entered password against the hashed password stored in the database.
            var passwordResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.Password,
                dto.Password
            );

            // If the password is incorrect, reject the login.
            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            // ----------------------------------------------------
            // Generate JWT token after successful authentication.
            // ----------------------------------------------------
            // Generate the short-lived JWT access token.
            var accessToken = GenerateJwtToken(user);

            // Generate a cryptographically secure refresh token.
            var refreshToken = GenerateRefreshToken();

            // Hash the refresh token before storing it in the database.
            // The actual token will be returned to the client,
            // but only its hash will be stored in SQL Server.
            var refreshTokenHash = _refreshTokenHasher.Hash(refreshToken);

            // Create the refresh token database record.
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,

                // Store only the hashed refresh token.
                Token = refreshTokenHash,

                CreatedOn = DateTime.UtcNow,

                // Refresh token is valid for 7 days.
                ExpiresOn = DateTime.UtcNow.AddDays(7)
            };

            // Save the refresh token in SQL Server.
            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            // Read token expiration setting from appsettings.json.
            var expirationMinutes =
                int.Parse(
                    _configuration["Jwt:AccessTokenExpirationMinutes"]!
                );

            // ----------------------------------------------------
            // Return token information to the controller.
            // ----------------------------------------------------
            return new LoginResponseDto
            {
                AccessToken = accessToken,

                // Return the refresh token to the client.
                RefreshToken = refreshToken,

                ExpiresInMinutes = expirationMinutes,
                Role = user.Role
            };
        }


        public async Task<LoginResponseDto?> RefreshTokenAsync(
       RefreshTokenDto dto)
        {
            // Hash the refresh token received from the client.
            // The database stores only the hashed refresh token.
            var refreshTokenHash = _refreshTokenHasher.Hash(dto.RefreshToken);

            // Find the hashed refresh token in the database.
            var storedToken = await _refreshTokenRepository
                .GetByTokenAsync(refreshTokenHash);

            // If the token does not exist, reject the request.
            if (storedToken == null)
            {
                return null;
            }

            // Check whether the refresh token has already expired.
            if (storedToken.ExpiresOn <= DateTime.UtcNow)
            {
                return null;
            }

            // Check whether the refresh token was already revoked.
            if (storedToken.RevokedOn != null)
            {
                return null;
            }

            // Revoke the old refresh token.
            // This prevents the same refresh token from being reused.
            storedToken.RevokedOn = DateTime.UtcNow;

            await _refreshTokenRepository.UpdateAsync(storedToken);

            // Generate a new access token for the same user.
            var accessToken = GenerateJwtToken(storedToken.User);

            // Generate a completely new refresh token.
            var newRefreshToken = GenerateRefreshToken();

            // Hash the new refresh token before storing it.
            var newRefreshTokenHash =
                _refreshTokenHasher.Hash(newRefreshToken);

            // Store the new hashed refresh token.
            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = storedToken.UserId,
                Token = newRefreshTokenHash,
                CreatedOn = DateTime.UtcNow,

                // New refresh token is valid for 7 days.
                ExpiresOn = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokenRepository
                .AddAsync(newRefreshTokenEntity);

            // Read access token expiration from configuration.
            var expirationMinutes = int.Parse(
                _configuration["Jwt:AccessTokenExpirationMinutes"]!
            );

            // Return the actual new refresh token to the client.
            // Only its hash is stored in the database.
            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresInMinutes = expirationMinutes,
                Role = storedToken.User.Role
            };
        }

        // --------------------------------------------------------
        // Creates the JWT token.
        // --------------------------------------------------------
        private string GenerateJwtToken(User user)
        {
            // Read secret key from appsettings.json.
            var key = _configuration["Jwt:Key"]!;

            // Convert the secret key into a security key.
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)
            );

            // Create signing credentials.
            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );

            // ----------------------------------------------------
            // Claims represent information about the authenticated
            // user that will be stored inside the JWT.
            // ----------------------------------------------------
            var claims = new List<Claim>
            {
                // User's unique database Id.
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                // Username.
                new Claim(
                    ClaimTypes.Name,
                    user.Username),

                // User's role.
                new Claim(
                    ClaimTypes.Role,
                    user.Role)
            };

            // ----------------------------------------------------
            // Create the JWT token.
            // ----------------------------------------------------
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(
                        _configuration[
                            "Jwt:AccessTokenExpirationMinutes"
                        ]!
                    )
                ),
                signingCredentials: credentials
            );

            // Convert JWT object into a string.
            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
        private string GenerateRefreshToken()
        {
            // Create 64 cryptographically secure random bytes.
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            // Convert the random bytes into a Base64 string.
            return Convert.ToBase64String(randomBytes);
        }
    }
}