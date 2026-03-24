using Basic.Models;
using Basic.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Basic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase    
    {
        //static List<Product> products = new();   //can we written as new  List<Product>();

        private readonly ProductService _productService;

        public ProductsController()
        {
            _productService = new ProductService();
        }



        //using the ActionResult for the control over http status code...
        [HttpGet]
        public ActionResult<List<Product>> GetAllProducts()
        {
            //return Ok(products);

            return Ok(_productService.GetAllProducts());

        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(string id)
        {
            // var product = products.FirstOrDefault(x => x.Id == id);
            
            var product = _productService.GetProductById(id);

            if (product == null) return NotFound();
            return Ok(product);

        }



        [HttpPost]
        public ActionResult<Product> CreateProduct([FromBody] Product product)
        {   
            //products.Add(product);

            _productService.CreateProduct(product);

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            //used this to provide the newly create resource url....
        }

        [HttpPut("{id}")]
        public ActionResult UpdateProduct( string id ,  [FromBody] Product newProduct) {

            //finding the product by id
            //var product = products.FirstOrDefault(p => p.Id == id);

            var product = _productService.GetProductById(id);

            if(product == null) return NotFound();      //404 returning if the product not found

            //updating the fields
            //product.Name = newProduct.Name;
            //product.Description = newProduct.Description;
            //product.Price = newProduct.Price;


            newProduct.Id = product.Id; //ensuring the same id 

            _productService.UpdateProduct(id, newProduct);


            return NoContent();



        }


        [HttpDelete("{id}")]
        public ActionResult DeleteProduct(string id)
        {
            //var product = products.FirstOrDefault(p => p.Id == id);

            var product = _productService.GetProductById(id);

            if (product == null) return NotFound();


            //products.Remove(product);

            _productService.DeleteProduct(id);

            return NoContent();
        }

    }
}
