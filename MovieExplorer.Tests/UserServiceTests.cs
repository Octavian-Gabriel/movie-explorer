using MovieExplorer.Data;
using MovieExplorer.Models;
using MovieExplorer.Models.ViewModels;
using MovieExplorer.Services;

namespace MovieExplorer.Tests
{
    public class UserServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly MovieExplorerDbContext _dbContext;

        public UserServiceTests(DatabaseFixture databaseFixture)
        {

            _dbContext = databaseFixture.dbContext;
        }

        [Fact]
        public async Task RegisterAsync_SuccessfulRegistration_ReturnsUser()
        {
            // Arrange
            var userService = new UserService(_dbContext);
            var userRegister = new RegisterViewModel
            {
                UserName = "testuser9",
                Password = "password123",
                Email = "test9@example.com"
            };
            // Act
            var user = await userService.RegisterAsync(userRegister);

            // Assert
            Assert.NotNull(user);
            Assert.Equal("test9@example.com", user.Email);
            Assert.Equal("testuser9", user.UserName);
            Assert.True(PasswordHasher.VerifyPassword("password123", user.PasswordHash));
        }

        [Fact]
        public async Task RegisterAsync_DuplicateEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            _dbContext.Users.Add(new User { Email = "test@example.com", UserName = "existinguser", PasswordHash = "hash" });
            await _dbContext.SaveChangesAsync();

            var userService = new UserService(_dbContext);
            var userRegister = new RegisterViewModel
            {
                UserName = "newuser",
                Password = "password123",
                Email = "test@example.com"
            };
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                userService.RegisterAsync(userRegister));
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var password = "password123";
            var passwordHash = PasswordHasher.HashPasword(password);
            _dbContext.Users.Add(new User { Email = "test12@example.com", UserName = "testuser12", PasswordHash = passwordHash });
            await _dbContext.SaveChangesAsync();

            var userService = new UserService(_dbContext);

            // Act
            var user = await userService.LoginAsync("test12@example.com", password);

            // Assert
            Assert.NotNull(user);
            Assert.Equal("test12@example.com", user.Email);
            Assert.Equal("testuser12", user.UserName);
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
            await Assert.ThrowsAsync<BCrypt.Net.SaltParseException>(() =>
                userService.LoginAsync("test@example.com", "wrongpassword"));
        }
    }
}