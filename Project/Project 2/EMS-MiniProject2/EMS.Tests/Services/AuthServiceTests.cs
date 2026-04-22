using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Models;
using EMS.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace EMS.Tests.Services
{
    [TestFixture]
    public class AuthServiceTests
    {
        private AppDbContext    _db      = null!;
        private AuthService     _service = null!;
        private IConfiguration  _config  = null!;

        [SetUp]
        public void Setup()
        {
            // Use InMemory database for auth tests
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _db = new AppDbContext(options);

            // Seed a test admin user
            _db.AppUsers.Add(new AppUser
            {
                Id           = 1,
                Username     = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role         = "Admin",
                CreatedAt    = DateTime.UtcNow
            });
            _db.SaveChanges();

            // Mock IConfiguration for JWT
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Jwt:Key"])         .Returns("TestSecretKey_32CharsMin_ForNUnit!!");
            mockConfig.Setup(c => c["Jwt:Issuer"])      .Returns("EMS.API");
            mockConfig.Setup(c => c["Jwt:Audience"])    .Returns("EMS.Client");
            mockConfig.Setup(c => c["Jwt:ExpiryHours"]).Returns("8");

            _config  = mockConfig.Object;
            _service = new AuthService(_db, _config);
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        // ── LoginAsync ────────────────────────────────────────────────────────

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsSuccessWithToken()
        {
            // Arrange
            var dto = new LoginRequestDto { Username = "admin", Password = "admin123" };

            // Act
            var result = await _service.LoginAsync(dto);

            // Assert
            Assert.That(result.Success,  Is.True);
            Assert.That(result.Token,    Is.Not.Null.And.Not.Empty);
            Assert.That(result.Username, Is.EqualTo("admin"));
            Assert.That(result.Role,     Is.EqualTo("Admin"));
        }

        [Test]
        public async Task LoginAsync_WrongPassword_ReturnsFailure()
        {
            // Arrange
            var dto = new LoginRequestDto { Username = "admin", Password = "wrongpass" };

            // Act
            var result = await _service.LoginAsync(dto);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Token,   Is.Null);
            Assert.That(result.Message, Does.Contain("Invalid"));
        }

        [Test]
        public async Task LoginAsync_NonExistentUser_ReturnsFailure()
        {
            // Arrange
            var dto = new LoginRequestDto { Username = "ghost", Password = "pass123" };

            // Act
            var result = await _service.LoginAsync(dto);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Token,   Is.Null);
        }

        [Test]
        public async Task LoginAsync_CaseInsensitiveUsername_ReturnsSuccess()
        {
            // Arrange — "ADMIN" should match "admin"
            var dto = new LoginRequestDto { Username = "ADMIN", Password = "admin123" };

            // Act
            var result = await _service.LoginAsync(dto);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Token,   Is.Not.Null.And.Not.Empty);
        }

        // ── RegisterAsync ─────────────────────────────────────────────────────

        [Test]
        public async Task RegisterAsync_NewUser_CreatesAccountSuccessfully()
        {
            // Arrange
            var dto = new RegisterRequestDto
            {
                Username = "newviewer",
                Password = "viewer123",
                Role     = "Viewer"
            };

            // Act
            var result = await _service.RegisterAsync(dto);

            // Assert
            Assert.That(result.Success,  Is.True);
            Assert.That(result.Username, Is.EqualTo("newviewer"));
            Assert.That(result.Role,     Is.EqualTo("Viewer"));

            // Verify persisted in DB
            var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Username == "newviewer");
            Assert.That(user,                  Is.Not.Null);
            Assert.That(user!.Role,            Is.EqualTo("Viewer"));
            // Verify password is hashed (not plain text)
            Assert.That(user.PasswordHash,     Is.Not.EqualTo("viewer123"));
            Assert.That(BCrypt.Net.BCrypt.Verify("viewer123", user.PasswordHash), Is.True);
        }

        [Test]
        public async Task RegisterAsync_DuplicateUsername_ReturnsConflictError()
        {
            // Arrange — "admin" already exists in seed data
            var dto = new RegisterRequestDto
            {
                Username = "admin",
                Password = "somepassword",
                Role     = "Admin"
            };

            // Act
            var result = await _service.RegisterAsync(dto);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Does.Contain("already exists"));
        }

        [Test]
        public async Task RegisterAsync_DefaultsToViewerRole_WhenRoleOmitted()
        {
            // Arrange
            var dto = new RegisterRequestDto
            {
                Username = "norolegiven",
                Password = "pass12345",
                Role     = "" // empty — should default to Viewer
            };

            // Act
            var result = await _service.RegisterAsync(dto);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Role,    Is.EqualTo("Viewer"));
        }

        // ── Token generation ──────────────────────────────────────────────────

        [Test]
        public async Task LoginAsync_ReturnedToken_IsNonEmptyJwtString()
        {
            // Arrange
            var dto = new LoginRequestDto { Username = "admin", Password = "admin123" };

            // Act
            var result = await _service.LoginAsync(dto);

            // Assert — JWT has 3 base64 sections separated by dots
            Assert.That(result.Token, Is.Not.Null);
            var parts = result.Token!.Split('.');
            Assert.That(parts.Length, Is.EqualTo(3));
        }
    }
}
