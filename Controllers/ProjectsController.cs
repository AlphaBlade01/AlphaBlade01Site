using AlphaBlade01.Logic.Contexts;
using AlphaBlade01.Logic.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

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
            using ProjectsContext db = new();
            ImmutableArray<ProjectModel> projects = db.Set<ProjectModel>().ToImmutableArray();

            return View("Index", projects);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(FormProjectsModel httpModel)
        {
            if (!ModelState.IsValid) return View();
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
                using Stream fileStream = new FileStream($"{_hostingEnvironment.ContentRootPath}/UploadedFiles/ProjectThumbnails/{model.Id}", FileMode.Create);
                await httpModel.Thumbnail.CopyToAsync(fileStream);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public FileResult GetProjectThumbnail(int id)
        {
            string filePath = _hostingEnvironment.ContentRootPath + "/UploadedFiles/ProjectThumbnails/" + id;
            return new FileStreamResult(new FileStream(filePath, FileMode.Open), "image/*");
        }
    }
}
