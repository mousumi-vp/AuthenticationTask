using AuthenticationTask.DTO;
using AuthenticationTask.Identity;
using AuthenticationTask.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationTask.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AuthenticationResponse CreateTwtToken(ApplicationUser user)
        {
            DateTime expiration =DateTime.UtcNow.AddMinutes
                (Convert.ToDouble(_configuration
                ["Jwt:EXPIRATION_MINUTES"]));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.NameIdentifier,user.Email),
                new Claim(ClaimTypes.Name,user.PersonName)

            };
            SymmetricSecurityKey security = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            SigningCredentials signingCredentials = new SigningCredentials
                (security, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expiration,
                signingCredentials: signingCredentials
                );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string token=tokenHandler.WriteToken(tokenGenerator);
            return new AuthenticationResponse()
            {
                Token = token,
                Email = user.Email,
                PersonName = user.PersonName,
                Expiration = expiration,
            };
            }
        }
    }
