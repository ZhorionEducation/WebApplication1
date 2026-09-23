namespace WebApplication1.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Documento { get; set; }
        public string Correo { get; set; } = string.Empty;
        public bool TarjetaPrestamo { get; set; }
        public int? LibroId { get; set; }
    }
}
