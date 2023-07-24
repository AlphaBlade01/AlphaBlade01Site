#nullable disable
using System.ComponentModel.DataAnnotations;

namespace AlphaBlade01.Logic.Models.DTOs
{
	public enum ProjectType
	{
		Models,
		Programming,
		Design
	}

	public class ProjectDTO
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

	public class InputProjectModel : ProjectDTO
	{
		[Required(ErrorMessage = "Thumbnail is required")]
		public IFormFile Thumbnail { get; set; }

		public ProjectDTO CastToModel()
		{
			return new ProjectDTO() { Id = Id, Name = Name, Description = Description, DateUploaded = DateUploaded, ProjectType = ProjectType };
		}
	}
}
