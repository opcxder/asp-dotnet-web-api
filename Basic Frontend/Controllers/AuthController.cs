using Basic_Frontend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Basic_Frontend.Controllers
{
    public class AuthController : Controller
    {

        private readonly HttpClient _httpClient;

        public AuthController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5202/api/");
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>  Login(Models.User user)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", user);
            
            if(response.IsSuccessStatusCode)
            {
                var token = (await response.Content.ReadAsStringAsync()).Trim('"');


                //storing token in the cookie;
                Response.Cookies.Append("jwt" , token , new CookieOptions
                {
                    Secure = false, // true when we using the https connection....
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict     //preventing in the cross site request...
                } );

                return RedirectToAction("Index", "Product");
            }
            ModelState.AddModelError("", "Invalid Login ");
            return View(user);
        }



        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");

            return RedirectToAction("Login", "Auth");
        }


        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {

            try
            {
                // trim input
                user.Username = user.Username?.Trim();
                user.Password = user.Password?.Trim();
                var response = await _httpClient.PostAsJsonAsync("auth/register", user);

                if(response.IsSuccessStatusCode)
                {
                 return    RedirectToAction("Index", "Product");
                }else
                {
                    ModelState.AddModelError("", "Api error: " + response.StatusCode);
                }
            }
            catch(Exception ex) 
            {
                ModelState.AddModelError("", "Api error: " + ex.Message);
            }


            return View(user); 
        }
    }
}
