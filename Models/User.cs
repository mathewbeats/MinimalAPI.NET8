using System.Text.Json.Serialization;

namespace MinimalApiDatabase.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }

        [JsonIgnore]
        // Propiedad de navegación
        public ICollection<Order> Orders { get; set; }
    }

}
