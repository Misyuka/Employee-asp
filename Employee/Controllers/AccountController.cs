using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Employee.Models.Account;
using Employee.Data;

namespace Employee.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login()
		{
			return View();
		}

        [HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginViewModel user)
		{
			if (ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);

                if (result.Succeeded)
				{
					ApplicationUser authUser = _userManager.Users.Where(x => x.UserName == user.Email).FirstOrDefault();
					if(authUser.Role == 0)
					{
						return RedirectToAction("Index", "Home");
					} else
					{
						return RedirectToAction("Index", "Manager");
					}

				}

				ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

			}
			return View(user);
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();

			return RedirectToAction("Login");
		}
	}
}
