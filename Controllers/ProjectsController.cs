using AlphaBlade01.Logic.Contexts;
using AlphaBlade01.Logic.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Collections.Immutable;
using System.Web;

namespace AlphaBlade01.Controllers
{
	public class ProjectsController : Controller
    {
        private readonly string _uploadedFilesPath;
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;

        public ProjectsController(IHostEnvironment environment, ApplicationDbContext context) 
        { 
            _hostingEnvironment = environment;
            _context = context;
            _uploadedFilesPath = $"{_hostingEnvironment.ContentRootPath}/UploadedFiles";
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

            ViewBag.Previews = Directory.EnumerateFiles($"{_uploadedFilesPath}/ProjectPreviews/{id}").Select(p => Path.GetFileName(p)).ToArray();
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
                using Stream fileStream = new FileStream($"{_uploadedFilesPath}/ProjectThumbnails/{model.Id}", FileMode.Create);
                await httpModel.Thumbnail.CopyToAsync(fileStream);
            }

            if (httpModel.Previews is not null && httpModel.Previews.Count() > 0)
            {
                DirectoryInfo previewDirectory = Directory.CreateDirectory($"{_uploadedFilesPath}/ProjectPreviews/{model.Id}");
                foreach (IFormFile preview in httpModel.Previews)
                {
                    using Stream fileStream = new FileStream($"{previewDirectory.FullName}/{preview.FileName}", FileMode.Create);
                    await preview.CopyToAsync(fileStream);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public FileResult GetProjectThumbnail(int id)
        {
            string filePath = _uploadedFilesPath + "/ProjectThumbnails/" + id;
            return new FileStreamResult(new FileStream(filePath, FileMode.Open), "image/*");
        }

        [HttpGet]
        public FileResult GetProjectPreview(int id, string fileName)
        {
            Console.WriteLine("{0} {1}", id, fileName);
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType);
            string filePath = $"{_uploadedFilesPath}/ProjectPreviews/{id}/{fileName}";
            return new FileStreamResult(new FileStream(filePath, FileMode.Open), contentType ?? "image");
        }
    }
}
