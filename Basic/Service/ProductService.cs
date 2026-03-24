using Basic.Models;
using MongoDB.Driver;

namespace Basic.Service
{
    public class ProductService
    {
        private readonly IMongoCollection<Product> _products;

        public ProductService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("ProductDB");
            _products = database.GetCollection<Product>("Products");
        }

        // Get all products
        public List<Product> GetAllProducts()
        {
            return _products.Find(p => true).ToList();   //here true means  all the products in the database
        }

        // Get product by id
        public Product GetProductById(string id)
        {
           return _products.Find(p => p.Id == id).FirstOrDefault();   
        }

        // Create product
        public void CreateProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.Id))
            {
                product.Id = Guid.NewGuid().ToString();
            }
            _products.InsertOne(product);
           
        }

        // Update product
        public void UpdateProduct(string id, Product product)
        {
            _products.ReplaceOne(p => p.Id == id, product);  
        }

        // Delete product
        public void DeleteProduct(string id)
        {
           _products.DeleteOne(p =>p.Id == id);
        }
    }
}