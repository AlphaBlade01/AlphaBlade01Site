using Microsoft.AspNetCore.Identity;

namespace AlphaBlade01.Logic.Models.DTOs
{
	public enum Role
	{
		ADMIN=1,
		GUEST=2
	}

	public class RoleDTO : IdentityRole<long>
	{
	}
}
