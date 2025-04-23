using Microsoft.EntityFrameworkCore;
using MovieExplorer.Data;
using MovieExplorer.Models;
using MovieExplorer.Services;
using Xunit;

namespace MovieExplorer.Tests
{
    public class UserServiceTests
    {
        private readonly MovieExplorerDbContext _dbContext;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<MovieExplorerDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _dbContext = new MovieExplorerDbContext(options);
        }

        [Fact]
        public async Task RegisterAsync_SuccessfulRegistration_ReturnsUser()
        {
            // Arrange
            var userService = new UserService(_dbContext);

            // Act
            var user = await userService.RegisterAsync( "testuser", "password123", "test@example.com");

            // Assert
            Assert.NotNull(user);
            Assert.Equal("test@example.com", user.Email);
            Assert.Equal("testuser", user.UserName);
            Assert.True(PasswordHasher.VerifyPassword("password123", user.PasswordHash));
        }

        [Fact]
        public async Task RegisterAsync_DuplicateEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            _dbContext.Users.Add(new User { Email = "test@example.com", UserName = "existinguser", PasswordHash = "hash" });
            await _dbContext.SaveChangesAsync();

            var userService = new UserService(_dbContext);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                userService.RegisterAsync("newuser", "password123", "test@example.com"));
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var password = "password123";
            var passwordHash = PasswordHasher.HashPasword(password);
            _dbContext.Users.Add(new User { Email = "test@example.com", UserName = "testuser", PasswordHash = passwordHash });
            await _dbContext.SaveChangesAsync();

            var userService = new UserService(_dbContext);

            // Act
            var user = await userService.LoginAsync("test@example.com", password);

            // Assert
            Assert.NotNull(user);
            Assert.Equal("test@example.com", user.Email);
            Assert.Equal("testuser", user.UserName);
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ThrowsInvalidOperationException()
        {
            // Arrange
            var password = "password123";
            var passwordHash = PasswordHasher.HashPasword(password);
            _dbContext.Users.Add(new User { Email = "test@example.com", UserName = "testuser", PasswordHash = passwordHash });
            await _dbContext.SaveChangesAsync();

            var userService = new UserService(_dbContext);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                userService.LoginAsync("test@example.com", "wrongpassword"));
        }
    }
}