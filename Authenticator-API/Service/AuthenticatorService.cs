using Authenticator_API.Model;
using Authenticator_API.Service.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Authenticator_API.Service
{
    public class AuthenticatorService : IAuthenticatorService
    {
        private readonly IConfiguration _config;

        private Dictionary<string, string> UserDB = new Dictionary<string, string>
        {
            {"Test@123","Test123" }
        };

        public AuthenticatorService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(string userName, string passWord)
        {
            var jwtAppSettingsSection = _config.GetSection("JWT");
            var jwtAppSettings = jwtAppSettingsSection.Get<JWTAppSettings>();
            var userExists = UserDB.Any(user => user.Key == userName &&
                                        user.Value == passWord);

            if (!userExists)
            {
                return "You are not a registered User. Please complete the registration!!!";
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtAppSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity
                (
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, userName)
                    }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                     SecurityAlgorithms.HmacSha256)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return token;
        }
    }
}