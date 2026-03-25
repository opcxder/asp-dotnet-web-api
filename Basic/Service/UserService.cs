using Basic.Models;

using MongoDB.Driver;
using BCrypt.Net;

namespace Basic.Service
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("ProductDB");
            _users = database.GetCollection<User>("Users");

        }

        //hashed password 
        public String HashPassword(string password)
        {
            string passwordHashed = BCrypt.Net.BCrypt.HashPassword(password);

            return passwordHashed;

        }

        //verify password
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }



        //add method to find  user by username...
        public async Task<User?> GetUserByUsername(string username)
        {
            return await _users.Find(user => user.Username == username).FirstOrDefaultAsync();
        }

        //create user
        public async Task CreateUser(User user)
        {
            user.Password  = HashPassword(user.Password);
            await _users.InsertOneAsync(user);
        }
    }
}
