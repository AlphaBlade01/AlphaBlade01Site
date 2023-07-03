using AlphaBlade01.Logic.Contexts;
using AlphaBlade01.Logic.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlphaBlade01.Controllers
{
    public class ProjectsController : Controller
    {
        private IHostEnvironment _hostingEnvironment;

        public ProjectsController(IHostEnvironment environment) 
        { 
            _hostingEnvironment = environment;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(FormProjectsModel httpModel)
        {
            ProjectModel model = httpModel.CastToModel();

            if (model.DateUploaded == default)
                model.DateUploaded = DateTime.Now;

            using (var db = new ProjectsContext())
            {
                db.Add(model);
                db.SaveChanges();
            }

            if (httpModel.Thumbnail is not null)
            {
                string fileExtension = Path.GetExtension(httpModel.Thumbnail.FileName);
                using Stream fileStream = new FileStream($"{_hostingEnvironment.ContentRootPath}/wwwroot/assets/images/projects/{model.Id}{fileExtension}", FileMode.Create);
                await httpModel.Thumbnail.CopyToAsync(fileStream);
            }

            return View();
        }
    }
}
