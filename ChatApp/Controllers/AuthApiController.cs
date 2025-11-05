using ChatApp.DTOs;
using ChatApp.JWTTOEkn.Interface;
using ChatApp.Models;
using ChatApp.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
    //    private readonly IUserRepository _userRepository;
    //    private readonly IJwtTokenService _jwt;
    //    private readonly IPasswordHasher<User> _passwordHasher;

    //    public AuthApiController(IUserRepository userRepo, IJwtTokenService jwt, IPasswordHasher<User> passwordHasher)
    //    {
    //        _userRepository = userRepo;
    //        _jwt = jwt;
    //        _passwordHasher = passwordHasher;
    //    }

    //    // Register User
    //    [HttpPost("register")]
    //    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    //    {
    //        if (!ModelState.IsValid)
    //            return BadRequest("Invalid input");

    //        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
    //            return BadRequest("Username and Password are required");

    //        var exists = await _userRepository.ExistAsync(request.UserName);
    //        if (exists)
    //            return Conflict("Username already exists");

    //        var user = new User
    //        {
    //            UserName = request.UserName.Trim()
    //        };
    //        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

    //        await _userRepository.CreateAsync(user);
    //        return Ok(new { message = "User registered successfully!" });
    //    }

    //    [HttpPost("login")]
    //    public async Task<IActionResult> Login([FromBody] LoginRequestDto logRequest)
    //    {
    //        if (string.IsNullOrWhiteSpace(logRequest.UserName) || string.IsNullOrWhiteSpace(logRequest.Password))
    //            return BadRequest("Username and Password are required");

    //        var user = await _userRepository.GetByUserNameAsync(logRequest.UserName.Trim());
    //        if (user == null)
    //            return Unauthorized("Invalid credentials");

    //        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, logRequest.Password);
    //        if (result == PasswordVerificationResult.Failed)

    //            return Unauthorized("Invalid credentials");

    //        var token = _jwt.CreateToken(user.UserName);

    //        return Ok(new
    //        {
    //            Token = token,
    //            Username = user.UserName
    //        });
    //    }
    }
}
