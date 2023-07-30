using AlphaBlade01.Logic.Contexts;
using AlphaBlade01.Logic.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

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

		public async Task<IActionResult> Index()
		{
			string? userName = User.Identity?.Name;
			if (userName is null) return View();

			UserDTO? user = await _userManager.FindByNameAsync(userName);
			if (user == null) return View();

			string role = (await _userManager.GetRolesAsync(user))[0];

			ViewBag.UserName = userName;
			ViewBag.Role = role;

			return View();
		}

		public IActionResult Register()
		{
			if ((bool)HttpContext.User.Identity?.IsAuthenticated) return Redirect("/");
			return View();
		}

		public IActionResult Login()
		{
			if ((bool)HttpContext.User.Identity?.IsAuthenticated) return Redirect("/");
			ViewData["Error"] = TempData["Error"];
			return View();
		}

		public IActionResult AccessDenied()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(InputUserModel userDto)
		{
			if (!ModelState.IsValid) return RedirectToAction(nameof(Register));
			if ((bool)HttpContext.User.Identity?.IsAuthenticated) return Redirect("/");

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

		[HttpPost]
		public async Task<IActionResult> Login(InputUserModel userDto)
		{
			if (!ModelState.IsValid) return RedirectToAction(nameof(Login));
			if ((bool)HttpContext.User.Identity?.IsAuthenticated) return Redirect("/");

			UserDTO? user = await _userManager.FindByNameAsync(userDto.UserName);
			if (user == null | !await _userManager.CheckPasswordAsync(user, userDto.Password))
			{
				TempData["Error"] = "Incorrect username or password";
				return RedirectToAction("Login");
			}

			await _signInManager.SignInAsync(user, isPersistent: false);
			return Redirect("/");
		}

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return Redirect("/");
		}

	}
}
