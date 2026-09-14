using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using SDTechnicalAssessment.Application.DTOs;

using Xunit;

namespace SDTechnicalAssessment.API.Tests
{
    public class ProductsControllerTests :
        IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ProductsControllerTests(
            WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetProducts_ShouldReturnUnauthorized_WhenTokenIsMissing()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/Products");

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.Unauthorized,
                response.StatusCode
            );
        }



        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var client = _factory.CreateClient();

            var loginRequest = new
            {
                Username = "admin",
                Password = "WrongPassword"
            };

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/Auth/login",
                loginRequest
            );

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.Unauthorized,
                response.StatusCode
            );
        }


        [Fact]
        public async Task Login_ShouldReturnTokens_WhenCredentialsAreValid()
        {
            // Arrange
            var client = _factory.CreateClient();

            var loginRequest = new
            {
                Username = "admin",
                Password = "Admin@123"
            };

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/Auth/login",
                loginRequest
            );

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.OK,
                response.StatusCode
            );

            var responseBody = await response.Content.ReadFromJsonAsync<LoginResponseTestDto>();

            Assert.NotNull(responseBody);
            Assert.False(string.IsNullOrEmpty(responseBody.AccessToken));
            Assert.False(string.IsNullOrEmpty(responseBody.RefreshToken));
            Assert.Equal("Admin", responseBody.Role);
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
        {
            // Arrange
            var client = _factory.CreateClient();

            // First login to get a valid refresh token.
            var loginRequest = new
            {
                Username = "admin",
                Password = "Admin@123"
            };

            var loginResponse = await client.PostAsJsonAsync(
                "/api/Auth/login",
                loginRequest
            );

            var loginData =
                await loginResponse.Content.ReadFromJsonAsync<LoginResponseTestDto>();

            Assert.NotNull(loginData);

            var refreshRequest = new
            {
                RefreshToken = loginData.RefreshToken
            };

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/Auth/refresh",
                refreshRequest
            );

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.OK,
                response.StatusCode
            );

            var refreshData =
                await response.Content.ReadFromJsonAsync<LoginResponseTestDto>();

            Assert.NotNull(refreshData);
            Assert.False(string.IsNullOrEmpty(refreshData.AccessToken));
            Assert.False(string.IsNullOrEmpty(refreshData.RefreshToken));
            Assert.Equal("Admin", refreshData.Role);
        }


        [Fact]
        public async Task RefreshToken_ShouldReturnUnauthorized_WhenOldTokenIsReused()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Login and get the first refresh token.
            var loginResponse = await client.PostAsJsonAsync(
                "/api/Auth/login",
                new
                {
                    Username = "admin",
                    Password = "Admin@123"
                });

            var loginData =
                await loginResponse.Content.ReadFromJsonAsync<LoginResponseTestDto>();

            Assert.NotNull(loginData);

            var oldRefreshToken = loginData.RefreshToken;

            // Use the refresh token once.
            var firstRefreshResponse = await client.PostAsJsonAsync(
                "/api/Auth/refresh",
                new
                {
                    RefreshToken = oldRefreshToken
                });

            Assert.Equal(
                System.Net.HttpStatusCode.OK,
                firstRefreshResponse.StatusCode
            );

            // Act - Try to use the OLD refresh token again.
            var secondRefreshResponse = await client.PostAsJsonAsync(
                "/api/Auth/refresh",
                new
                {
                    RefreshToken = oldRefreshToken
                });

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.Unauthorized,
                secondRefreshResponse.StatusCode
            );
        }


        [Fact]
        public async Task GetProducts_ShouldReturnOk_WhenValidTokenIsProvided()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Login first to get a valid access token.
            var loginResponse = await client.PostAsJsonAsync(
                "/api/Auth/login",
                new
                {
                    Username = "admin",
                    Password = "Admin@123"
                });

            var loginData =
                await loginResponse.Content.ReadFromJsonAsync<LoginResponseTestDto>();

            Assert.NotNull(loginData);

            // Add JWT token to the Authorization header.
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    loginData.AccessToken
                );

            // Act
            var response = await client.GetAsync("/api/v1/Products");

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.OK,
                response.StatusCode
            );
        }

      
        [Fact]
        public async Task GetProductById_ShouldReturnOk_WhenValidTokenIsProvided()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Login as Admin.
            var loginResponse = await client.PostAsJsonAsync(
                "/api/Auth/login",
                new
                {
                    Username = "admin",
                    Password = "Admin@123"
                });

            var loginData =
                await loginResponse.Content.ReadFromJsonAsync<LoginResponseTestDto>();

            Assert.NotNull(loginData);

            // Add Admin JWT token.
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    loginData.AccessToken
                );

            // Create a product first.
            // This makes the test independent of an existing Product ID.
            var createRequest = new
            {
                ProductName = "Product For Get Test",
                CreatedBy = "admin"
            };

            var createResponse = await client.PostAsJsonAsync(
                "/api/v1/Products",
                createRequest
            );

            Assert.Equal(
                System.Net.HttpStatusCode.Created,
                createResponse.StatusCode
            );

            // Read the created product to get its actual database ID.
            var createdProduct =
                await createResponse.Content.ReadFromJsonAsync<ProductDto>();

            Assert.NotNull(createdProduct);

            // Act
            var response = await client.GetAsync(
                $"/api/v1/Products/{createdProduct.Id}"
            );

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.OK,
                response.StatusCode
            );
        }

        [Fact]
        public async Task GetProductById_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Login to get a valid JWT token.
            var loginResponse = await client.PostAsJsonAsync(
                "/api/Auth/login",
                new
                {
                    Username = "admin",
                    Password = "Admin@123"
                });

            var loginData =
                await loginResponse.Content.ReadFromJsonAsync<LoginResponseTestDto>();

            Assert.NotNull(loginData);

            // Add JWT token to Authorization header.
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    loginData.AccessToken
                );

            // Act
            // 999999 should not exist in our database.
            var response = await client.GetAsync(
                "/api/v1/Products/999999");

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.NotFound,
                response.StatusCode
            );
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnCreated_WhenAdminTokenIsProvided()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Login as Admin.
            var loginResponse = await client.PostAsJsonAsync(
                "/api/Auth/login",
                new
                {
                    Username = "admin",
                    Password = "Admin@123"
                });

            var loginData =
                await loginResponse.Content.ReadFromJsonAsync<LoginResponseTestDto>();

            Assert.NotNull(loginData);

            // Add Admin JWT to Authorization header.
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    loginData.AccessToken
                );

            var createRequest = new
            {
                ProductName = "API Test Product",
                CreatedBy = "admin"
            };

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/v1/Products",
                createRequest
            );

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.Created,
                response.StatusCode
            );
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnForbidden_WhenUserTokenIsProvided()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Login as normal User.
            var loginResponse = await client.PostAsJsonAsync(
                "/api/Auth/login",
                new
                {
                    Username = "user",
                    Password = "User@123"
                });


            Assert.Equal(
    System.Net.HttpStatusCode.OK,
    loginResponse.StatusCode
);
            var loginData =
                await loginResponse.Content.ReadFromJsonAsync<LoginResponseTestDto>();

            Assert.NotNull(loginData);

            // Add User JWT to Authorization header.
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    loginData.AccessToken
                );

            var createRequest = new
            {
                ProductName = "User Product",
                CreatedBy = "user"
            };

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/v1/Products",
                createRequest
            );

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.Forbidden,
                response.StatusCode
            );
        }



     
        [Fact]
        public async Task UpdateProduct_ShouldReturnNoContent_WhenAdminTokenIsProvided()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Login as Admin.
            var loginResponse = await client.PostAsJsonAsync(
                "/api/Auth/login",
                new
                {
                    Username = "admin",
                    Password = "Admin@123"
                });

            var loginData =
                await loginResponse.Content.ReadFromJsonAsync<LoginResponseTestDto>();

            Assert.NotNull(loginData);

            // Add Admin JWT.
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    loginData.AccessToken
                );

            // First create a product.
            // This makes the test independent of existing database data.
            var createRequest = new
            {
                ProductName = "Product For Update Test",
                CreatedBy = "admin"
            };

            var createResponse = await client.PostAsJsonAsync(
                "/api/v1/Products",
                createRequest
            );

            Assert.Equal(
                System.Net.HttpStatusCode.Created,
                createResponse.StatusCode
            );

            // Read the created product and get its actual ID.
            var createdProduct =
                await createResponse.Content.ReadFromJsonAsync<ProductDto>();

            Assert.NotNull(createdProduct);

            var updateRequest = new
            {
                ProductName = "Updated API Product",
                ModifiedBy = "admin"
            };

            // Act
            // Use the ID generated by the database.
            var response = await client.PutAsJsonAsync(
                $"/api/v1/Products/{createdProduct.Id}",
                updateRequest
            );

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.NoContent,
                response.StatusCode
            );
        }


        [Fact]
        public async Task DeleteProduct_ShouldReturnNoContent_WhenAdminTokenIsProvided()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Login as Admin to get JWT token.
            var loginResponse = await client.PostAsJsonAsync(
                "/api/Auth/login",
                new
                {
                    Username = "admin",
                    Password = "Admin@123"
                });

            var loginData =
                await loginResponse.Content.ReadFromJsonAsync<LoginResponseTestDto>();

            Assert.NotNull(loginData);

            // Add Admin JWT token to the request.
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    loginData.AccessToken
                );

            // First create a product.
            // We do this so that the test does not depend on Product ID = 1.
            var createRequest = new
            {
                ProductName = "Product To Delete",
                CreatedBy = "admin"
            };

            var createResponse = await client.PostAsJsonAsync(
                "/api/v1/Products",
                createRequest
            );

            Assert.Equal(
                System.Net.HttpStatusCode.Created,
                createResponse.StatusCode
            );

            // Read the newly created product and get its ID.
            var createdProduct =
                await createResponse.Content.ReadFromJsonAsync<ProductDto>();

            Assert.NotNull(createdProduct);

            // Act
            var response = await client.DeleteAsync(
                $"/api/v1/Products/{createdProduct.Id}"
            );

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.NoContent,
                response.StatusCode
            );
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnForbidden_WhenUserTokenIsProvided()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Login as normal User to get a User-role JWT token.
            var loginResponse = await client.PostAsJsonAsync(
                "/api/Auth/login",
                new
                {
                    Username = "user",
                    Password = "User@123"
                });

            var loginData =
                await loginResponse.Content.ReadFromJsonAsync<LoginResponseTestDto>();

            Assert.NotNull(loginData);

            // Add User JWT token to the request.
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    loginData.AccessToken
                );

            // Act
            var response = await client.DeleteAsync(
                "/api/v1/Products/1"
            );

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.Forbidden,
                response.StatusCode
            );
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnBadRequest_WhenProductNameIsEmpty()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Login as Admin.
            var loginResponse = await client.PostAsJsonAsync(
                "/api/Auth/login",
                new
                {
                    Username = "admin",
                    Password = "Admin@123"
                });

            var loginData =
                await loginResponse.Content.ReadFromJsonAsync<LoginResponseTestDto>();

            Assert.NotNull(loginData);

            // Add Admin JWT token.
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    loginData.AccessToken
                );

            // ProductName is intentionally empty.
            // FluentValidation should reject this request.
            var invalidRequest = new
            {
                ProductName = "",
                CreatedBy = "admin"
            };

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/v1/Products",
                invalidRequest
            );

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.BadRequest,
                response.StatusCode
            );
        }


    }



    public class LoginResponseTestDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; }
        public string Role { get; set; } = string.Empty;
    }


}