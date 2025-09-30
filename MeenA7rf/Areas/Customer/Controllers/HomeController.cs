using Microsoft.AspNetCore.Mvc;

namespace MeenA7rf.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
