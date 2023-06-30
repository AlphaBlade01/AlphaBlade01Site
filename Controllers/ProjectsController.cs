using Microsoft.AspNetCore.Mvc;

namespace AlphaBlade01.Controllers
{
    public class ProjectsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
