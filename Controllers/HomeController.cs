using CourseManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseManager.Controllers;

public class HomeController : BaseController
{
    public HomeController(ICookieService cookieService) : base(cookieService) { }

    public IActionResult Index()
    {
        CookieDataSetup();

        return View();
    }
    public IActionResult Privacy()
    {
        CookieDataSetup();
        return View();
    }
}
