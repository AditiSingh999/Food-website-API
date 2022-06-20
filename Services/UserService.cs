using FoodWebsite.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoToken.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MongoToken.Services
{
    public class UserService
    {
        private readonly IMongoCollection<FWModel> _FWCol;
        private readonly IMongoCollection<User> _usersCollection;
        private readonly string Key;

        public UserService(
            IOptions<UserDatabaseSettings> userDatabaseSettings, IConfiguration configuration)
        {
            var mongoClient = new MongoClient(
                userDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                userDatabaseSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(
                userDatabaseSettings.Value.UsersCollectionName);

            _FWCol = mongoDatabase.GetCollection<FWModel>(
                userDatabaseSettings.Value.OrdersCollectionName);

            //for authentication
            this.Key = configuration.GetSection("JwtKey").ToString();
        }

        public async Task<List<User>> GetAsync() =>
            await _usersCollection.Find(_ => true).ToListAsync();

        // authenticate method
        public string Authenticate(string email, string password)
        {

            var user = _usersCollection.Find(x => x.Email == email && x.Password == password).FirstOrDefault();

            if (user == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.ASCII.GetBytes(Key);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, email)
                }),

                Expires = DateTime.UtcNow.AddHours(1),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                    )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task CreateAsync(User newBook) =>
            await _usersCollection.InsertOneAsync(newBook);

        public async Task CreateAsync(FWModel newOrder) =>
            await _FWCol.InsertOneAsync(newOrder);

        public async Task<FWModel> GetAsync(string email) =>
            await _FWCol.Find(x => x.Email == email).FirstOrDefaultAsync();
    }
}
