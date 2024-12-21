//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace CharityHubOnionArchitecture.authConfig
{
  
    public static class SecurityConfig
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("SecurityConfig");

            // Add JWT authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Enable HTTPS in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // Adjust based on your needs
                    ValidateAudience = false, // Adjust based on your needs
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["auth:secretKey"])),
                    ClockSkew = TimeSpan.Zero // Disable clock skew
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        logger.LogError($"JWT Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        logger.LogInformation("JWT Token validated successfully");
                        return Task.CompletedTask;
                    }
                };
            });

            // Configure authorization policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AllowAll", policy => policy.RequireAssertion(_ => true)); // Example policy
            });

            // Add MVC services (if not already added)
            services.AddControllers();
        }

        public static void Configure(WebApplication app)
        {
            // Use authentication and authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Add route mappings
            app.MapControllers();

            // Permit actuator and public endpoints
            app.Map("/actuator", () => Results.Ok("Actuator is up and running!"));
            app.Map("/v1/accounts/authenticate", () => Results.Ok("Authenticate endpoint"));
            app.Map("/error", () => Results.Problem("An error occurred"));

            // Enforce authenticated access for other routes
            app.Use(async (context, next) =>
            {
                if (!context.User.Identity.IsAuthenticated && context.Request.Path != "/actuator" &&
                    context.Request.Path != "/v1/accounts/authenticate" &&
                    context.Request.Path != "/error")
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                await next();
            });
        }
    }
}
