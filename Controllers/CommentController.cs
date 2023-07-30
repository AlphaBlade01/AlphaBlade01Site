using AlphaBlade01.Logic.Contexts;
using AlphaBlade01.Logic.Models.DTOs;
using AlphaBlade01.Logic.ViewComponents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Security.Claims;

namespace AlphaBlade01.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserDTO> _userManager;

        public CommentController(ApplicationDbContext context, UserManager<UserDTO> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(string comment)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Unauthorized();

            CommentDTO commentDTO = new()
            {
                Author = await _userManager.FindByIdAsync(userId),
                Value = comment,
                Timestamp = DateTime.UtcNow
            };

            await _context.AddAsync(commentDTO);
            await _context.SaveChangesAsync();

            return Ok();
        } 
    }
}
