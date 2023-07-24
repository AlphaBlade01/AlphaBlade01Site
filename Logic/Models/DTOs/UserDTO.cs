#nullable disable

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AlphaBlade01.Logic.Models.DTOs
{
	public class UserDTO : IdentityUser<long>
    {
	}

    public class  InputUserModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
