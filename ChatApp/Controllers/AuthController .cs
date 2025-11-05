using ChatApp.DTOs;
using ChatApp.JWTTOEkn.Interface;
using ChatApp.Models;
using ChatApp.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    public class AuthController : Controller
    {

        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwt;
        private readonly IPasswordHasher<User> _passwordHasher;
        public AuthController(IUserRepository userRepo, IJwtTokenService jwt, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepo;
            _jwt = jwt;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
                return View(request);

            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                ModelState.AddModelError("", "Username and Password are required");
                return View(request);
            }

            var exists = await _userRepository.ExistAsync(request.UserName);
            if (exists)
            {
                ModelState.AddModelError("", "Username already taken..");
                return View(request);
            }

            var user = new User
            {
                UserName = request.UserName.Trim()   //why trim
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            await _userRepository.CreateAsync(user);

            TempData["Success"] = "Register successful! You can now Log In..";
            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginRequestDto logRequest)
        {

           if(string.IsNullOrWhiteSpace(logRequest.UserName) || string.IsNullOrWhiteSpace(logRequest.Password))
            {
                return BadRequest("UserName abd Password reqird");
            }


            var user = await _userRepository.GetByUserNameAsync(logRequest.UserName.Trim());
            if (user == null)
            {
                return BadRequest("Invalid username or password");
            }


            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, logRequest.Password);
            if (verify == PasswordVerificationResult.Failed)
            {
                return BadRequest("Invalid username or passwird.");
            }

            var token = _jwt.CreateToken(user.UserName);

            HttpContext.Session.SetString("AuthToken", token);
            HttpContext.Session.SetString("Username", user.UserName);

            return Json(new {success=true,token=token ,username=user.UserName});

        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
