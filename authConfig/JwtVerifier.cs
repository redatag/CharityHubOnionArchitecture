
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


namespace CharityHubOnionArchitecture.authConfig
{
    public class JwtVerifier
    {
        private readonly string _secretKey;

        public JwtVerifier(IConfiguration configuration)
        {
            _secretKey = configuration["auth:secretKey"];
        }

        public JwtSecurityToken Verify(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = GetSecretKey();

            try
            {
                var parameters = new TokenValidationParameters
                {
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Disables default 5 minute clock skew for JWT expiration
                };

                var claimsPrincipal = tokenHandler.ValidateToken(token, parameters, out var validatedToken);
                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    ValidateExpiry(jwtToken);
                    return jwtToken;
                }
                throw new UnauthorizedAccessException("Invalid JWT token");
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("JWT token verification failed", ex);
            }
        }

        private SymmetricSecurityKey GetSecretKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        }

        private void ValidateExpiry(JwtSecurityToken token)
        {
            if (token.ValidTo < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("JWT token expired");
            }
        }
    }

}