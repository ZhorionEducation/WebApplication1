using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Datos;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Documento,Correo,TarjetaPrestamo")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Documento,Correo,TarjetaPrestamo")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Usuario/Prestar/5
        //este get para poder mostrar la vista de prestamo y poder seleccionar el libro que se va a prestar
        public async Task<IActionResult> Prestar(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id.Value);
            if (usuario == null) return NotFound();

            // Libros no prestados
            ViewBag.Libros = new SelectList(_context.Libros.Where(b => !b.Prestado).ToList(), "Id", "Titulo");
            return View(usuario);
        }


        //este post es para cuando se envia el formulario
        [HttpPost, ActionName("Prestar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Prestar(int id, int libroId)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            var libro = await _context.Libros.FindAsync(libroId);

            if (usuario == null) return NotFound();

            if (libro == null)
            {
                ModelState.AddModelError("libroId", "Libro no encontrado.");
                ViewBag.Libros = new SelectList(_context.Libros.Where(b => !b.Prestado).ToList(), "Id", "Titulo");
                return View(usuario);
            }

            if (!usuario.TarjetaPrestamo)
            {
                ModelState.AddModelError(string.Empty, "El usuario no tiene tarjeta disponible.");
                ViewBag.Libros = new SelectList(_context.Libros.Where(b => !b.Prestado).ToList(), "Id", "Titulo");
                return View(usuario);
            }

            // Protección ante condición de carrera: comprobar de nuevo el estado del libro
            if (libro.Prestado)
            {
                ModelState.AddModelError("libroId", "El libro ya está prestado.");
                ViewBag.Libros = new SelectList(_context.Libros.Where(b => !b.Prestado).ToList(), "Id", "Titulo");
                return View(usuario);
            }

            // Marcar prestado y guardar
            usuario.TarjetaPrestamo = false;
            usuario.LibroId = libro.Id; 
            libro.Prestado = true;
            _context.Update(usuario);
            _context.Update(libro);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Libro prestado correctamente.";
            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Devolver(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound();

            if (usuario.LibroId.HasValue)
            {
                var libro = await _context.Libros.FindAsync(usuario.LibroId.Value);

                if (libro != null)
                {
                    libro.Prestado = false;
                }
            }

            usuario.TarjetaPrestamo = true;
            usuario.LibroId = null;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Libro devuelto correctamente.";

            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
