using SDTechnicalAssessment.Infrastructure.Security;

namespace SDTechnicalAssessment.Infrastructure.Tests
{
    public class RefreshTokenHasherTests
    {
        [Fact]
        public void Hash_ShouldReturnSameHash_ForSameToken()
        {
            // Arrange
            // Create the class we want to test.
            var hasher = new RefreshTokenHasher();

            var token = "test-refresh-token";

            // Act
            // Hash the same token twice.
            var hash1 = hasher.Hash(token);
            var hash2 = hasher.Hash(token);

            // Assert
            // Same input should always produce the same hash.
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void Hash_ShouldReturnDifferentHash_ForDifferentTokens()
        {
            // Arrange
            var hasher = new RefreshTokenHasher();

            var token1 = "refresh-token-1";
            var token2 = "refresh-token-2";

            // Act
            var hash1 = hasher.Hash(token1);
            var hash2 = hasher.Hash(token2);

            // Assert
            // Different tokens should produce different hashes.
            Assert.NotEqual(hash1, hash2);
        }
    }
}