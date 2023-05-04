using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Threading.Tasks;

public class CultureMiddleware
{
    private readonly RequestDelegate _next;

    public CultureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        // 세션에서 언어 설정 가져오기
        string cultureName = httpContext.Session.GetString("culture") ?? null;

        // 세션에 언어 설정이 없으면 기본값으로 "en-US" 사용
        if (string.IsNullOrEmpty(cultureName))
        {
            cultureName = "en-US";
        }

        // CultureInfo 생성 및 설정
        CultureInfo culture = new CultureInfo(cultureName);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        // 다음 미들웨어 호출
        await _next(httpContext);
    }
}

public static class CultureMiddlewareExtensions
{
    public static IApplicationBuilder UseCultureMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CultureMiddleware>();
    }
}
