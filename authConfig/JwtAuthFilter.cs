

using CharityHubOnionArchitecture.common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;
namespace CharityHubOnionArchitecture.authConfig
{
    public class JwtAuthFilter
    {
        private readonly JwtVerifier _jwtVerifier;
        private readonly ILogger<JwtAuthFilter> _log;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public JwtAuthFilter(JwtVerifier jwtVerifier, ILogger<JwtAuthFilter> log)
        {
            _jwtVerifier = jwtVerifier;
            _log = log;
            _jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                string token = ResolveTokenFromAuthHeader(context.Request);

                if (token == null)
                {
                    await next(context);
                    return;
                }

                var claims = _jwtVerifier.Verify(token);
                _log.LogInformation("Token verified successfully");

                var payload = AccessTokenPayload.FromPayload(claims);
                var claimsIdentity = new ClaimsIdentity(
                    payload.Permissions.Select(permission => new Claim(ClaimTypes.Role, permission)),
                    "Bearer"
                );

                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                context.User = claimsPrincipal;

                await next(context);
            }
            catch (Exception ex)
            {
                _log.LogError("Error processing JWT token", ex);
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var errorResponse = new Dictionary<string, string>
            {
                { "description", "Invalid token" }
            };

                var jsonResponse = JsonConvert.SerializeObject(errorResponse, _jsonSerializerSettings);
                await context.Response.WriteAsync(jsonResponse);
            }
        }

        private string ResolveTokenFromAuthHeader(HttpRequest request)
        {
            var authorizationHeader = request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authorizationHeader.Substring("Bearer ".Length).Trim();
            }
            return null;
        }
    }
}
