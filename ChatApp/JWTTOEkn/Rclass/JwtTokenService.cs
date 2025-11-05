using ChatApp.JWTTOEkn.Interface;
using ChatApp.JWTTOEkn.ModelJwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatApp.JWTTOEkn.Rclass
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _Setting;
        private readonly byte[] _KeyBytes;
        public JwtTokenService(IOptions<JwtSettings>option) 
        {
            _Setting = option.Value;
            _KeyBytes=Encoding.UTF8.GetBytes(_Setting.Key);
        }
        public string CreateToken(string username)
        {
            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.Sub,username),
               new Claim(ClaimTypes.Name, username)
             };

            var creds=new SigningCredentials(new SymmetricSecurityKey(_KeyBytes),SecurityAlgorithms.HmacSha256);
            var now=DateTime.UtcNow;


            var token = new JwtSecurityToken(
                issuer: _Setting.Issuer,
                audience: _Setting.Audience,
                claims:claims,
                notBefore:now,
                expires:now.AddMinutes(_Setting.ExpMinutes),
                signingCredentials:creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
