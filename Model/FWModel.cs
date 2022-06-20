using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FoodWebsite.Models
{
    public class FWModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Order> Orders { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public class Order
        {
            public int Id { get; set; }
            public string Item { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }
    }
}