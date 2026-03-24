using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

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


        //get products
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Models.Product>? products = new List<Models.Product>();

            try
            {

                var response = await _httpClient.GetAsync("products");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<IEnumerable<Models.Product>>();

                    if (result != null)
                    {
                        products = result;
                    }else
                    {
                        ModelState.AddModelError("" , "API return empty response" );
                    }
                } else
                {
                    ModelState.AddModelError("", "API error: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to fetch data from the api,  API Error " + ex.Message);
            }



            return View(products );
        }



        [HttpGet]
        //show the form
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        //create product 
        public async Task<IActionResult> Create(Models.Product product)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("products", product);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "API failed: " + response.StatusCode);
                }
            }catch(Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
            }

            return View(product);     //in case of error, form should retain the entered value, other wise user have to add again information 
        }



        //delete the data from the database
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _httpClient.DeleteAsync($"products/{id}");

            return RedirectToAction("Index");
        }


        //edit the database
        [HttpGet]
        public  async Task<IActionResult>  Edit(string id)
        {
            var response = await _httpClient.GetAsync($"Products/{id}");

            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadFromJsonAsync<Models.Product>();
                return View(product);
            }
            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> Edit (Models.Product product)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"products/{product.Id}", product);
                if(response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Update failed");
            }catch(Exception ex)
            {
                ModelState.AddModelError("", "Error : ," +  ex.Message);
            }
            return View(product);
        }


    
    }

}
