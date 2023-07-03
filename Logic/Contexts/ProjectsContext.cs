using AlphaBlade01.Logic.Models;
using Microsoft.EntityFrameworkCore;

namespace AlphaBlade01.Logic.Contexts
{
    public class ProjectsContext : DbContext
    {
        public DbSet<ProjectModel> Projects { get; set; }

        public ProjectsContext() { }
        public ProjectsContext(DbContextOptions options): base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite(@"Data Source=Data/projects.db");
    }
}
