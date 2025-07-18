namespace CourseManager.Services;
public interface ICookieService
{
    void SetFirstVisitCookie();
    string GetFirstVisitDate();
}
public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CookieKey = "FirstVisit";

    public CookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetFirstVisitCookie()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return;

        if (!context.Request.Cookies.ContainsKey(CookieKey))
        {
            context.Response.Cookies.Append(CookieKey, DateTime.Now.ToString("f"), new CookieOptions
            {
                Expires = DateTime.Now.AddYears(1),
                IsEssential = true
            });
        }
    }

    public string GetFirstVisitDate()
    {
        var context = _httpContextAccessor.HttpContext;
        return context?.Request.Cookies[CookieKey];
    }
}