using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Basic_Frontend.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5202/api/");
        }

        // helper method to add jwt token from cookie to header
        [NonAction]
        private void AuthJwtToken()
        {
            var token = Request.Cookies["jwt"];

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // get all products
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Models.Product> products = new List<Models.Product>();

            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            try
            {
                AuthJwtToken();
                var response = await _httpClient.GetAsync("products");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<IEnumerable<Models.Product>>();
                    if (result != null)
                        products = result;
                    else
                        ModelState.AddModelError("", "API returned empty response");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Auth");
                }
                else
                {
                    ModelState.AddModelError("", "API error: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to fetch data from API. " + ex.Message);
            }

            return View(products);
        }

        // show create product form
        [HttpGet]
        public IActionResult Create()
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            return View();
        }

        // create product
        [HttpPost]
        public async Task<IActionResult> Create(Models.Product product)
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            try
            {
                AuthJwtToken();
                var response = await _httpClient.PostAsJsonAsync("products", product);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");

                ModelState.AddModelError("", "API failed: " + response.StatusCode);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
            }

            return View(product); // retain entered values in case of error
        }

        // delete product by id
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            try
            {
                AuthJwtToken();
                var response = await _httpClient.DeleteAsync($"products/{id}");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return RedirectToAction("Login", "Auth");
            }
            catch
            {
                // optional: log error
            }

            return RedirectToAction("Index");
        }

        // show edit form
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            AuthJwtToken();
            var response = await _httpClient.GetAsync($"products/{id}");

            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadFromJsonAsync<Models.Product>();
                return View(product);
            }

            return RedirectToAction("Index"); // fallback if not found
        }

        // update product
        [HttpPost]
        public async Task<IActionResult> Edit(Models.Product product)
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            try
            {
                AuthJwtToken();
                var response = await _httpClient.PutAsJsonAsync($"products/{product.Id}", product);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");

                ModelState.AddModelError("", "Update failed");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
            }

            return View(product); // retain values in case of error
        }
    }
}