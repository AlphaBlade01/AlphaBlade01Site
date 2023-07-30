#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaBlade01.Logic.Models.DTOs
{
    public class CommentDTO
    {
        public int Id { get; set; }
        [Required]
        public long AuthorId { get; set; }
        public UserDTO Author { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
    }
}
