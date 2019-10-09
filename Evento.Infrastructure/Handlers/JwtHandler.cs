using Evento.Infrastructure.DTO;
using Evento.Infrastructure.Extensions;
using Evento.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Evento.Infrastructure.Handlers
{
    public class JwtHandler : IJwtHandler
    {
        private readonly JwtSettings _jwtSettings;

        public JwtHandler(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public JwtDto CreateToken(Guid userId, string role)
        {
            // to przerobić jak w  tutorialu dev for sidney

            var now = DateTime.UtcNow;
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
                /// rola
                new Claim(ClaimTypes.Role, role),
                /// unikany indentyfikator tokenu
                new Claim(JwtRegisteredClaimNames.Jti, userId.ToString()),
                /// data wydania tokenu
                new Claim(JwtRegisteredClaimNames.Iat, now.ToTimestamp().ToString())
            };

            var expires = now.AddMinutes(_jwtSettings.ExpiryMinutes);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)), SecurityAlgorithms.HmacSha256Signature);

            var jwt = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                claims: claims,
                expires: expires,
                notBefore: now,
                signingCredentials:
                signingCredentials
                );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new JwtDto
            {
                Token = token,
                Expires = expires.ToTimestamp()
            };

        }
    }
}
