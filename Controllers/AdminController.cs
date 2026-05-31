using Microsoft.AspNetCore.Mvc;

namespace EcommerceCartSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}