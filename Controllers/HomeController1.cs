using Microsoft.AspNetCore.Mvc;

namespace Proyectonext.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
