using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniAuthMvc.Data;
using MiniAuthMvc.Models;
using System.Security.Claims;

namespace MiniAuthMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly PasswordHasher<AppUser> _hasher = new();

        public AccountController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var username = model.Username.Trim();
            var usernameKey = username.ToLowerInvariant();
            var email = model.Email.Trim().ToLowerInvariant();

            var usernameExists = await _db.Users.AnyAsync(u => u.Username.ToLower() == usernameKey);
            if (usernameExists)
            {
                ModelState.AddModelError(nameof(model.Username), "Username already exists.");
                return View(model);
            }

            var emailExists = await _db.Users.AnyAsync(u => u.Email.ToLower() == email);
            if (emailExists)
            {
                ModelState.AddModelError(nameof(model.Email), "Email already exists.");
                return View(model);
            }

            var user = new AppUser
            {
                Username = username,
                Email = email,
                PhoneNumber = model.PhoneNumber.Trim()
            };

            user.PasswordHash = _hasher.HashPassword(user, model.Password);

            try
            {
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Handles race-condition duplicates (unique index violation)
                ModelState.AddModelError("", "Username or Email already exists. Try another.");
                return View(model);
            }

            await SignInUserAsync(user, rememberMe: true); // usually ok after register
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var key = model.UsernameOrEmail.Trim().ToLowerInvariant();

            var user = await _db.Users.FirstOrDefaultAsync(u =>
                u.Username.ToLower() == key || u.Email.ToLower() == key);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username/email or password.");
                return View(model);
            }

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Invalid username/email or password.");
                return View(model);
            }

            await SignInUserAsync(user, model.RememberMe);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private async Task SignInUserAsync(AppUser user, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var props = new AuthenticationProperties
            {
                IsPersistent = rememberMe
            };

            // If rememberMe = true, keep cookie longer; otherwise it becomes session cookie
            if (rememberMe)
                props.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
