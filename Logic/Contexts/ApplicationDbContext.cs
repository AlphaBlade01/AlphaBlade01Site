using AlphaBlade01.Logic.Models;
using AlphaBlade01.Logic.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlphaBlade01.Logic.Contexts
{
	public partial class ApplicationDbContext : IdentityDbContext<UserDTO, RoleDTO, long>
	{
		private readonly IConfiguration _configuration;

		public ApplicationDbContext(IConfiguration configuration) 
		{
			_configuration = configuration;
		}
		public ApplicationDbContext(DbContextOptions options, IConfiguration configuration) : base(options) 
		{
			_configuration = configuration;
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<RoleDTO>(entity =>
			{
				entity.HasData(
					new RoleDTO
					{
						Id = 1,
						Name = "Admin",
						NormalizedName = "ADMIN"
					},
					new RoleDTO
					{
						Id = 2,
						Name = "Guest",
						NormalizedName = "GUEST"
					});
			});
		}
		
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			=> optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
	}

	
	public partial class ApplicationDbContext
	{
		public DbSet<ProjectDTO> Projects { get; set; }
	}

	public partial class ApplicationDbContext
	{
		public DbSet<CommentDTO> Comments { get; set; }
	}
}
