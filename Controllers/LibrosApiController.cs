using WebApplication1.Datos;
using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    [Route("api/libros")]
    [ApiController]
    public class LibrosApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LibrosApiController(AppDbContext context)
        {
            _context = context;
        }
        // GET: api/LibrosApi
        [HttpGet]
        public async Task<IActionResult> GetLibros()
        {
            var libros = await _context.Libros.Include(l => l.Autor).Include(l => l.Categoria).ToListAsync();
            return Ok(libros);
        }

        // GET: api/LibrosApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLibro(int id)
        {
            var libro = await _context.Libros.Include(l => l.Autor).Include(l => l.Categoria).FirstOrDefaultAsync(l => l.Id == id);
            if (libro == null)
            {
                return NotFound();
            }
            return Ok(libro);
        }

        // POST: api/LibrosApi
        [HttpPost]
        public async Task<IActionResult> CrearLibro(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLibro), new { id = libro.Id }, libro);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarLibro(int id, Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != libro.Id)
            {
                return BadRequest();
            }
            var libroExistente = await _context.Libros.FindAsync(id);
            if (libroExistente == null)
            {
                return NotFound();
            }
            libroExistente.Titulo = libro.Titulo;
            libroExistente.AutorId = libro.AutorId;
            await _context.SaveChangesAsync();
            return Ok(libroExistente);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarLibro(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }
            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();
            return Ok(libro);
        }
    }
}