using System.Text;
using Microsoft.IdentityModel.Tokens;
using OrchestrationService.Model.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace OrchestrationService.ApiExtensions;

public static partial class ApiExtensions
{
        public static void AddApiAuthentication(
                this IServiceCollection services,
                IConfiguration configuration)
        {
                var jwtOptions = configuration
                        .GetSection("JWT")
                        .Get<JwtOptions>();

                services.AddAuthentication(options =>
                        {
                                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        })
                        .AddJwtBearer(options =>
                        {
                                options.RequireHttpsMetadata = true;
                                options.SaveToken = true;

                                options.TokenValidationParameters = new()
                                {
                                        ValidateIssuer = false,
                                        ValidateAudience = false,
                                        ValidateLifetime = true,
                                        ValidateIssuerSigningKey = true,
                                        IssuerSigningKey = new SymmetricSecurityKey(
                                                Encoding.UTF8.GetBytes(jwtOptions!.SecretKey)),

                                        RoleClaimType = "role" 
                                };

                                options.Events = new JwtBearerEvents
                                {
                                        OnMessageReceived = context =>
                                        {
                                                var token = context.Request.Cookies["kubelab"];

                                                if (string.IsNullOrEmpty(token))
                                                {
                                                        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                                                        if (authHeader?.StartsWith("Bearer ") == true)
                                                                token = authHeader.Substring("Bearer ".Length);
                                                }

                                                context.Token = token;
                                                return Task.CompletedTask;
                                        }
                                };
                        });

                services.AddAuthorization(options =>
                {
                        options.AddPolicy("Admin", p => p.RequireRole("Admin"));
                        options.AddPolicy("Student", p => p.RequireRole("Student"));
                });
        }
}