using AlphaBlade01.Logic.Contexts;
using AlphaBlade01.Logic.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphaBlade01.Logic.ViewComponents
{
    [ViewComponent]
    public class CommentsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserDTO> _userManager;

        public CommentsViewComponent(ApplicationDbContext context, UserManager<UserDTO> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<CommentDTO> comments = await _context.Comments
                .OrderByDescending(c => c.Timestamp)
                .ToListAsync();

            Dictionary<int, string> commentAuthors = new();
            foreach (var comment in comments)
            {
                UserDTO user = await _userManager.FindByIdAsync(comment.AuthorId.ToString()) ?? new();
                commentAuthors.Add(comment.Id, user.UserName ?? string.Empty);
            }

            ViewBag.CommentAuthors = commentAuthors;
            return View(comments);
        }
    }
}
