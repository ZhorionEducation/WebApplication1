using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class Autor
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<Libro> Libros { get; set; } = new List<Libro>();
    }
}
