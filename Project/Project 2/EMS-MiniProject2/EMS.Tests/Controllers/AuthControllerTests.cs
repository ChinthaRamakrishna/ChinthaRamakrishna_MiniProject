using EMS.API.Controllers;
using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace EMS.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private AuthService    _authService = null!;
        private AuthController _controller  = null!;
        private AppDbContext   _db          = null!;

        [SetUp]
        public void Setup()
        {
            // InMemory database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _db = new AppDbContext(options);

            // Mock IConfiguration for JWT
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Jwt:Key"])         .Returns("TestSecretKey_32CharsMin_ForNUnit!!");
            mockConfig.Setup(c => c["Jwt:Issuer"])      .Returns("EMS.API");
            mockConfig.Setup(c => c["Jwt:Audience"])    .Returns("EMS.Client");
            mockConfig.Setup(c => c["Jwt:ExpiryHours"]).Returns("8");

            _authService = new AuthService(_db, mockConfig.Object);
            _controller  = new AuthController(_authService);
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        // ── POST /api/auth/register ───────────────────────────────────────────

        [Test]
        public async Task Register_NewUser_ReturnsOk()
        {
            // Arrange
            var dto = new RegisterRequestDto
            {
                Username = "newuser",
                Password = "pass123",
                Role     = "Viewer"
            };

            // Act
            var result = await _controller.Register(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok   = (OkObjectResult)result;
            var data = (AuthResponseDto)ok.Value!;
            Assert.That(data.Success,  Is.True);
            Assert.That(data.Username, Is.EqualTo("newuser"));
            Assert.That(data.Role,     Is.EqualTo("Viewer"));
        }

        [Test]
        public async Task Register_DuplicateUsername_ReturnsConflict()
        {
            // Arrange — register first time
            var dto = new RegisterRequestDto
            {
                Username = "duplicate",
                Password = "pass123",
                Role     = "Admin"
            };
            await _controller.Register(dto);

            // Act — register again with same username
            var result = await _controller.Register(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
            var conflict = (ConflictObjectResult)result;
            var data     = (AuthResponseDto)conflict.Value!;
            Assert.That(data.Success, Is.False);
            Assert.That(data.Message, Does.Contain("already exists"));
        }

        [Test]
        public async Task Register_AdminRole_CreatesAdminAccount()
        {
            // Arrange
            var dto = new RegisterRequestDto
            {
                Username = "newadmin",
                Password = "admin123",
                Role     = "Admin"
            };

            // Act
            var result = await _controller.Register(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok   = (OkObjectResult)result;
            var data = (AuthResponseDto)ok.Value!;
            Assert.That(data.Success, Is.True);
            Assert.That(data.Role,    Is.EqualTo("Admin"));
        }

        [Test]
        public async Task Register_EmptyRole_DefaultsToViewer()
        {
            // Arrange
            var dto = new RegisterRequestDto
            {
                Username = "noroleuser",
                Password = "pass123",
                Role     = ""
            };

            // Act
            var result = await _controller.Register(dto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok   = (OkObjectResult)result;
            var data = (AuthResponseDto)ok.Value!;
            Assert.That(data.Role, Is.EqualTo("Viewer"));
        }

        // ── POST /api/auth/login ──────────────────────────────────────────────

        [Test]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange — register first
            await _controller.Register(new RegisterRequestDto
            {
                Username = "testlogin",
                Password = "test123",
                Role     = "Admin"
            });

            var loginDto = new LoginRequestDto
            {
                Username = "testlogin",
                Password = "test123"
            };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok   = (OkObjectResult)result;
            var data = (AuthResponseDto)ok.Value!;
            Assert.That(data.Success,  Is.True);
            Assert.That(data.Token,    Is.Not.Null.And.Not.Empty);
            Assert.That(data.Username, Is.EqualTo("testlogin"));
            Assert.That(data.Role,     Is.EqualTo("Admin"));
        }

        [Test]
        public async Task Login_WrongPassword_ReturnsUnauthorized()
        {
            // Arrange — register first
            await _controller.Register(new RegisterRequestDto
            {
                Username = "wrongpass",
                Password = "correctpass",
                Role     = "Viewer"
            });

            var loginDto = new LoginRequestDto
            {
                Username = "wrongpass",
                Password = "wrongpassword"
            };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
            var unauth = (UnauthorizedObjectResult)result;
            var data   = (AuthResponseDto)unauth.Value!;
            Assert.That(data.Success, Is.False);
            Assert.That(data.Token,   Is.Null);
        }

        [Test]
        public async Task Login_NonExistentUser_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginRequestDto
            {
                Username = "ghostuser",
                Password = "anypass"
            };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        }

        [Test]
        public async Task Login_CaseInsensitiveUsername_ReturnsOk()
        {
            // Arrange — register lowercase
            await _controller.Register(new RegisterRequestDto
            {
                Username = "casetest",
                Password = "pass123",
                Role     = "Viewer"
            });

            // Act — login with UPPERCASE
            var result = await _controller.Login(new LoginRequestDto
            {
                Username = "CASETEST",
                Password = "pass123"
            });

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var ok   = (OkObjectResult)result;
            var data = (AuthResponseDto)ok.Value!;
            Assert.That(data.Success, Is.True);
            Assert.That(data.Token,   Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task Login_ReturnedToken_IsValidJwtFormat()
        {
            // Arrange
            await _controller.Register(new RegisterRequestDto
            {
                Username = "jwttest",
                Password = "jwt12345",
                Role     = "Admin"
            });

            // Act
            var result = await _controller.Login(new LoginRequestDto
            {
                Username = "jwttest",
                Password = "jwt12345"
            });

            // Assert — JWT has 3 parts separated by dots
            var ok    = (OkObjectResult)result;
            var data  = (AuthResponseDto)ok.Value!;
            var parts = data.Token!.Split('.');
            Assert.That(parts.Length, Is.EqualTo(3));
        }
    }
}
