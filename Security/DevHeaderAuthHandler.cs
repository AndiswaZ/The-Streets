using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace TheStreets_BE.Security
{
    /// <summary>
    /// Lightweight dev-time authentication that reads user identity from request headers.
    /// Headers:
    ///   - X-User-Id (required for write ops)
    ///   - X-User-Name (optional display name)
    ///   - X-User-Role (optional; e.g., 'SuperAdmin' to allow cross-user deletes)
    /// </summary>
    public sealed class DevHeaderAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        // ✅ .NET 8 constructor signature (no ISystemClock)
        public DevHeaderAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // If header is missing, we return NoResult() so anonymous endpoints still work
            var userId = Request.Headers["X-User-Id"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, Request.Headers["X-User-Name"].FirstOrDefault() ?? "Anonymous")
            };

            var role = Request.Headers["X-User-Role"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}