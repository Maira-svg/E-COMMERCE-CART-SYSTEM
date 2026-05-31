using Microsoft.AspNetCore.Mvc;

namespace EcommerceCartSystem.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}