using Authenticator_API.Model;
using Authenticator_API.Service.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Authenticator_API.Service
{
    public class AuthenticatorService : IAuthenticatorService
    {
        private readonly IConfiguration _config;
        private readonly SqlDataReader reader;
        private readonly SqlCommand sqlComm = new();
        private readonly SqlConnection sqlConn = new();
        private readonly List<string> Users = new();

        public AuthenticatorService(IConfiguration config)
        {
            _config = config;
            sqlConn.ConnectionString = config.GetConnectionString("EmployeeInfo");
            sqlConn.Open();
            sqlComm.Connection = sqlConn;
            sqlComm.CommandText = @"/****** Script for SelectTopNRows command from SSMS  ******/
                                    SELECT TOP (1000) [Id]
                                    ,[UserName]
                                    ,[PasswordHash]
                              FROM [Employee].[dbo].[AspNetUsers]";
            reader = sqlComm.ExecuteReader();
        }

        public async Task<string> GenerateTokenAsync(string userName)
        {
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    Users.Add(reader["UserName"].ToString());
                }
            }
            var jwtAppSettingsSection = _config.GetSection("JWT");
            var jwtAppSettings = jwtAppSettingsSection.Get<JWTAppSettings>();
            var userExists = Users.Any(user => user == userName);

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