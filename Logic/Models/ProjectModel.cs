#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;

namespace AlphaBlade01.Logic.Models
{
    public enum ProjectType
    {
        Models,
        Programming,
        Design
    }

    public class ProjectModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime DateUploaded { get; set; }
        [Required]
        public ProjectType ProjectType { get; set; }
    }

    public class FormProjectsModel : ProjectModel
    {
        [Required(ErrorMessage = "Thumbnail is required")]
        public IFormFile Thumbnail { get; set; }

        public ProjectModel CastToModel()
        {
            return new ProjectModel() { Id = Id, Name = Name, Description = Description, DateUploaded = DateUploaded, ProjectType = ProjectType };
        }
    }
}