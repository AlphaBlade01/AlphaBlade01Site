using AlphaBlade01.Logic.Contexts;
using AlphaBlade01.Logic.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AlphaBlade01.Controllers
{
	public class AccountController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<UserDTO> _userManager;
		private readonly SignInManager<UserDTO> _signInManager;
		public readonly IPasswordHasher<UserDTO> _passwordHasher;

		public AccountController(UserManager<UserDTO> userManager, SignInManager<UserDTO> signInManager, IPasswordHasher<UserDTO> passwordHasher, ApplicationDbContext context)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_passwordHasher = passwordHasher;
			_context = context;
		}

		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(InputUserModel userDto)
		{
			UserDTO user = new UserDTO();
			string hashedPassword = _passwordHasher.HashPassword(user, userDto.Password);

			user.UserName = userDto.UserName;
			user.PasswordHash = hashedPassword;
			user.SecurityStamp = Guid.NewGuid().ToString();

			_context.Users.Add(user);
			_context.SaveChanges();

			await _userManager.AddToRoleAsync(user, "GUEST");
			await _signInManager.SignInAsync(user, isPersistent: false);

			return Redirect("/");
		}

		public IActionResult Login()
		{
			if (HttpContext.User.Identity is not null) return Redirect("/");
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(InputUserModel userDto)
		{
			if (HttpContext.User.Identity is not null) return Redirect("/");

			UserDTO? user = await _userManager.FindByNameAsync(userDto.UserName);
			if (user == null | !await _userManager.CheckPasswordAsync(user, userDto.Password)) return RedirectToAction("Login");

			await _signInManager.SignInAsync(user, isPersistent: false);
			return Redirect("/");
		}
	}
}
