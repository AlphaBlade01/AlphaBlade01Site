#nullable disable
using System.Web.Helpers;

namespace AlphaBlade01.Logic.Models
{
    public class ProjectModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateUploaded { get; set; }
    }

    public class FormProjectsModel : ProjectModel
    {
        public IFormFile Thumbnail { get; set; }
        public ProjectModel CastToModel()
        {
            return new ProjectModel() { Id = Id, Name = Name, Description = Description };
        }
    }
}