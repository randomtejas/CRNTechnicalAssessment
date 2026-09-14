using System.Security.Cryptography;
using System.Text;

namespace SDTechnicalAssessment.Infrastructure.Security
{
    // This class is responsible for hashing refresh tokens
    // before storing them in the database.
    public class RefreshTokenHasher
    {
        public string Hash(string token)
        {
            // Convert the refresh token into bytes.
            var tokenBytes = Encoding.UTF8.GetBytes(token);

            // Create a SHA-256 hash of the token.
            var hashBytes = SHA256.HashData(tokenBytes);

            // Convert the hash into a string so it can be stored in SQL Server.
            return Convert.ToBase64String(hashBytes);
        }
    }
}