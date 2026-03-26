using Basic.Models;
using Basic.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Basic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {


        private readonly UserService _userService;
       
        private readonly IConfiguration _configuration;

        public AuthController(UserService userService , IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {

            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest(" Username  and password are required");
            }


            var existingUser = await _userService.GetUserByUsername(user.Username);
            if (existingUser != null)
            {
                return BadRequest("User already register");
            }
            else
            {
                await _userService.CreateUser(user);
                return Ok("User registered successfully");
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest("Username and Password cannot be empty");
            }

            var existingUser = await _userService.GetUserByUsername(user.Username);

            if (existingUser == null)
            {
                return Unauthorized("Invalid username or password");
            }

            if (!_userService.VerifyPassword(user.Password, existingUser.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            var token = GenerateJwtToken(existingUser);

            return Ok(token);
        }
   
       
         private string GenerateJwtToken(User user)
        {
            
            //creating the claims 
            var claims = new[] {
                 new Claim(ClaimTypes.NameIdentifier , user.Id),
                 new Claim(ClaimTypes.Name ,  user.Username)
            };

            //this is used when we have implemented the symetric signing 
            //var key =   new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            //var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var rsa = AuthService.LoadRsaKey();
            var key = new RsaSecurityKey(rsa);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),
                signingCredentials: credentials

                );
            return new JwtSecurityTokenHandler().WriteToken(token);
            
        }
    
    }
}
