using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TestWebApp
{
    public class CustomAuthenticationMiddleware : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public CustomAuthenticationMiddleware(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer"))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                // DB에서 토큰 유효성 체크
                var user = await GetUserFromTokenAsync(token);
                if (user != null)
                {
                    // 인증 성공
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email),
                        // 기타 클레임 정보 추가
                    };
                    var identity = new ClaimsIdentity(claims, "jwt");
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return AuthenticateResult.Success(ticket);
                }
            }

            // 인증 실패
            return AuthenticateResult.Fail("Invalid Authorization header");
        }

        private async Task<User> GetUserFromTokenAsync(string token)
        {
            await Task.Delay(500);
            return new User { Name = token,Email="test@test.test" };
        }
    }
}
