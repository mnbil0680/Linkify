using Microsoft.AspNetCore.Mvc;

namespace LinkifyPLL.Controllers
{
    public class JobController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
