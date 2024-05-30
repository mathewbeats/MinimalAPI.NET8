using System.Text.Json.Serialization;

namespace MinimalApiDatabase.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int UserId { get; set; }


        [JsonIgnore]
        // Propiedad de navegación
        public User User { get; set; }
    }


}
