using AlphaBlade01.Logic.Contexts;
using AlphaBlade01.Logic.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        private async Task SaveThumbnail(int id, IFormFile thumbnail)
        {
            using Stream fileStream = new FileStream($"{_uploadedFilesPath}/ProjectThumbnails/{id}", FileMode.Create);
            await thumbnail.CopyToAsync(fileStream);
        }

        private async Task SavePreviews(int id, IFormFile[] previews)
        {
            DirectoryInfo previewDirectory = Directory.CreateDirectory($"{_uploadedFilesPath}/ProjectPreviews/{id}");
            foreach (IFormFile preview in previews)
            {
                using Stream fileStream = new FileStream($"{previewDirectory.FullName}/{preview.FileName}", FileMode.Create);
                await preview.CopyToAsync(fileStream);
            }
        }

		public IActionResult Index()
        {
            ImmutableArray<ProjectDTO> projects = _context.Set<ProjectDTO>().OrderByDescending(p => p.DateUploaded).ToImmutableArray();
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

            Directory.CreateDirectory($"{_uploadedFilesPath}/ProjectPreviews/{id}");
            ViewBag.Previews = Directory.EnumerateFiles($"{_uploadedFilesPath}/ProjectPreviews/{id}").Select(p => Path.GetFileName(p)).ToArray();

			return View(project);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            ProjectDTO? project = await _context.Projects.FindAsync(id);

            if (project is null) 
                return NotFound();

            return View(new InputProjectModel(project));
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
                await SaveThumbnail(model.Id, httpModel.Thumbnail);

            if (httpModel.Previews is not null && httpModel.Previews.Count() > 0)
                await SavePreviews(model.Id, httpModel.Previews);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(InputProjectModel inputProjectModel)
        {
            ProjectDTO? project = await _context.FindAsync<ProjectDTO>(inputProjectModel.Id);

            if (project == null) return NotFound();

            if (inputProjectModel.DateUploaded == default)
                inputProjectModel.DateUploaded = DateTime.Now.ToUniversalTime();

            project.Name = inputProjectModel.Name;
            project.Description = inputProjectModel.Description;
            project.DateUploaded = inputProjectModel.DateUploaded;
            project.ProjectType = inputProjectModel.ProjectType;

            _context.SaveChanges();

            if (inputProjectModel.Thumbnail is not null)
                await SaveThumbnail(project.Id, inputProjectModel.Thumbnail);

            if (inputProjectModel.Previews is not null && inputProjectModel.Previews.Count() > 0)
                await SavePreviews(project.Id, inputProjectModel.Previews);

            return RedirectToAction("View", new { id = inputProjectModel.Id });
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
