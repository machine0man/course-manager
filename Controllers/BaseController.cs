using CourseManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseManager.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ICookieService _cookieService;
        public BaseController(ICookieService cookieService)
        {
            _cookieService = cookieService;
        }
        /// <summary>
        /// is being used to call the two methods from this methods wherever neede in the base classes,
        /// i am calling in every get method in controllers so cookie setup hapens on first visit of any webpage 
        /// </summary>
        protected void CookieDataSetup()
        {
            SetFirstVisitCookie();
            SetFirstVisitInViewBag();
        }
        protected void SetFirstVisitCookie()
        {
            _cookieService.SetFirstVisitCookie();
        }
        protected void SetFirstVisitInViewBag()
        {
            ViewBag.FirstVisit = _cookieService.GetFirstVisitDate();
        }
    }
}