using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Swar.API.Interfaces.Services;
using Swar.API.Models.DBModels;
using Swar.API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Swar.UnitTest.ServiceUnitTest
{
    public class TokenServiceTests
    {
        private ITokenService _tokenService;
        private IConfiguration _configuration;
        private Mock<ILogger<TokenService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"TokenKey:Access", "08adeaf6148022445c28b37a1a8bf67806a73bcd6a30fd3ad41136f37fcab65d"},
                {"TokenKey:Refresh", "08adeaf6148022445c28b37a1a8bf67806a73bcd6a30fd3ad41136f37fcab65d"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _loggerMock = new Mock<ILogger<TokenService>>();
            _tokenService = new TokenService(_configuration, _loggerMock.Object);
        }

        [Test]
        public void GenerateToken_ValidUser_ReturnsToken()
        {
            // Arrange
            var user = new User
            {
                UserId = 123,
                Name = "Test User",
                Email = "user@example.com",
                Gender = "Male",
            };

            // Act
            var accessToken = _tokenService.GenerateJwtToken(user, "access");
            var refreshToken = _tokenService.GenerateJwtToken(user, "refresh");


            // Assert
            Assert.NotNull(accessToken);
            Assert.NotNull(refreshToken);

            // Verify token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("08adeaf6148022445c28b37a1a8bf67806a73bcd6a30fd3ad41136f37fcab65d");
            tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userIdClaim = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var roleClaim = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;

            Assert.That(userIdClaim, Is.EqualTo("123"));
            Assert.That(roleClaim, Is.EqualTo("User"));
        }
    }
}