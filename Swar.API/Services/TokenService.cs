﻿using Microsoft.IdentityModel.Tokens;
using Swar.API.Interfaces.Services;
using Swar.API.Models.DBModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Swar.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _accessTokenSecret;
        private readonly string _refreshTokenSecret;
        private readonly SymmetricSecurityKey _accessTokenKey;
        private readonly SymmetricSecurityKey _refreshTokenKey;
        private readonly TimeSpan _accessTokenExpiration = TimeSpan.FromHours(5);
        private readonly TimeSpan _refreshTokenExpiration = TimeSpan.FromDays(30);
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _accessTokenSecret = configuration["TokenKey:Access"];
            _refreshTokenSecret = configuration["TokenKey:Refresh"];
            _accessTokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessTokenSecret));
            _refreshTokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refreshTokenSecret));
            _logger = logger;
        }

        public string GenerateJwtToken(User user, string token_type)
        {
            var claims = CreateClaims(user, token_type);
            TimeSpan expiration = token_type == "access" ? _accessTokenExpiration : _refreshTokenExpiration;
            var key = token_type == "access" ? _accessTokenKey : _refreshTokenKey;


            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "https://go-swar.us.auth0.com/",
                audience: "https://swarapi.azurewebsites.net/api/v1/",
                claims: claims,
                expires: DateTime.UtcNow.Add(expiration),
                signingCredentials: credentials
            );

            _logger.LogInformation($"Token generated for user {user.UserId} with token type {token_type}");
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private List<Claim> CreateClaims(User user, string token_type)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("token_type", token_type)
            };
        }
    }
}
