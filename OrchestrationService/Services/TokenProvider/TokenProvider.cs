using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using OrchestrationService.Domain;
using OrchestrationService.Model.Tokens;

namespace OrchestrationService.Services.TokenProvider;

public class TokenProvider(IOptions<TokenOptions> tokenOptions) : ITokenProvider
{
        public AccessToken CreateAccessToken(User user)
        {
                var signingKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(tokenOptions.Value.SecretKey));

                var signingCredentials = new SigningCredentials(
                        signingKey,
                        SecurityAlgorithms.HmacSha512);

                var claims = new List<Claim>
                {
                        new Claim("sub", user.PublicId),
                        new Claim("name", user.Username),
                        new Claim("role", user.Role.ToString())
                };

                var token = new JwtSecurityToken(
                        issuer: tokenOptions.Value.Issuer,
                        audience: tokenOptions.Value.Audience,
                        claims: claims,
                        expires: DateTime.UtcNow.AddHours(tokenOptions.Value.ExpireHours),
                        signingCredentials: signingCredentials);

                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                return new AccessToken
                        { Role = (int)user.Role, Token = jwtToken, ExpiresAt = token.ValidTo, UserId = user.PublicId };
        }

        public RefreshToken CreateRefreshToken(User user)
        {
                var tokenId = Guid.NewGuid().ToString();
                var expires = DateTime.UtcNow.AddDays(tokenOptions.Value.RefreshTokenLifetimeDays);

                return new RefreshToken
                {
                        Token = tokenId,
                        UserId = user.PublicId,
                        ExpiresAt = expires,
                        IsRevoked = false
                };
        }
}