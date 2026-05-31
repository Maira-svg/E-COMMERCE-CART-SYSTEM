using Microsoft.AspNetCore.Mvc;

namespace EcommerceCartSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}