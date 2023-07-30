using AlphaBlade01.Logic.Contexts;
using AlphaBlade01.Logic.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

namespace AlphaBlade01.Controllers
{
	public class ProjectsController : Controller
    {
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;

        public ProjectsController(IHostEnvironment environment, ApplicationDbContext context) 
        { 
            _hostingEnvironment = environment;
            _context = context;
        }


        public IActionResult Index()
        {
            ImmutableArray<ProjectDTO> projects = _context.Set<ProjectDTO>().ToImmutableArray();
            return View("Index", projects);
        }

        public async Task<IActionResult> View(int id)
        {
            ProjectDTO? project = await _context.Projects.FindAsync(id);

            if (project is null)
            {
                ViewData["Error"] = "No such project exists in the database.";
                return RedirectToAction("Index");
            }

            return View(project);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(InputProjectModel httpModel)
        {
            if (!ModelState.IsValid) return View();
            ProjectDTO model = httpModel.CastToModel();

            if (model.DateUploaded == default)
                model.DateUploaded = DateTime.Now.ToUniversalTime();

            _context.Add(model);
            _context.SaveChanges();

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
