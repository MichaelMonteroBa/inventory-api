using Inventory.Api.Data;
using Inventory.Api.Models;
using Microsoft.AspNetCore.Mvc; // acceso a clase ASP.NET
using Microsoft.EntityFrameworkCore;
using Inventory.Api.DTOs;
using MySqlConnector;
// investigar para que sirven estos using

namespace Inventory.Api.Controllers;

// Define la ruta del controlador del contexto de la base de datos
[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
   private readonly ApplicationDbContext _context;
   // Inyeccion de dependencia del contexto de la base de datos
   public CategoriesController(ApplicationDbContext context)
   {
      _context = context;
   }
   
   // Obtiene una lista de todas las categorias
   [HttpGet]
   public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
   {
      // Select proyecta en SQL solo las columnas necesarias, sin traer la entidad completa
      return await _context.Categories
         .Select(c => new CategoryDto
         {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
         })
         .ToListAsync();
   }
    
   // Obtiene una categoria especifica por su ID
   [HttpGet("{id}")]
   public async Task<ActionResult<CategoryDto>> GetCategory(Guid id)
   {
      var category = await _context.Categories.FindAsync(id);
      if (category == null)
      {
         return NotFound();
      }
      return ToDto(category);
   }
   
   // Crea una nueva categoria
   [HttpPost]
   public async Task<ActionResult<CategoryDto>> PostCategory(CreateCategoryDto dto)
   {
      // 1. Normaliza: sin espacios al inicio ni al final
      var name = dto.Name.Trim();

      // 2. Comprobacion previa: AnyAsync no trae la fila, solo devuelve true/false.
      //    La comparacion la hace MySQL con el collation de la columna,
      //    la misma regla que usa el indice unico.
      if (await _context.Categories.AnyAsync(c => c.Name == name))
      {
         return Problem(
            detail: $"Ya existe una categoria con el nombre '{name}'.",
            statusCode: StatusCodes.Status409Conflict);
      }

      var category = new Category
      {
         Id = Guid.NewGuid(), // el servidor decide el id, nunca el cliente
         Name = name,
         Description = dto.Description // Description ya es nulable en la entidad
      };

      _context.Categories.Add(category);

      // 3. Guardado protegido: cubre la condicion de carrera, donde dos peticiones
      //    pasan la comprobacion previa y la segunda choca con el indice unico.
      try
      {
         await _context.SaveChangesAsync();
      }
      catch (DbUpdateException ex)
         when (ex.InnerException is MySqlException { ErrorCode: MySqlErrorCode.DuplicateKeyEntry })
      {
         // Solo el error 1062 (entrada duplicada) se traduce a 409.
         // Cualquier otro DbUpdateException no entra en este catch y sigue su camino normal.
         return Problem(
            detail: $"Ya existe una categoria con el nombre '{name}'.",
            statusCode: StatusCodes.Status409Conflict);
      }

      return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, ToDto(category));
   }
   
   // Actualiza una nueva categoria existente
   [HttpPatch("{id}")]
   public async Task<IActionResult> PatchCategory(Guid id, Category category)
   {
      if (id != category.Id)
      {
         return BadRequest();
      }

      _context.Entry(category).State = EntityState.Modified;
      try
      {
         await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
         if (!CategoryExists(id))
         {
            return NotFound();
         }
         else
         {
            throw;
         }
      }
      return NoContent();
   }
   
   // Elimina una categoria
   [HttpDelete("{id}")]
   public async Task<IActionResult> DeleteCategory(Guid id)
   {
      var category = await _context.Categories.FindAsync(id);
      if (category == null)
      {
         return NotFound();
      }

      _context.Categories.Remove(category);
      await _context.SaveChangesAsync();
      return NoContent();
   }

   private bool CategoryExists(Guid id)
   {
      return _context.Categories.Any(e => e.Id == id);
   }
   
   private static CategoryDto ToDto(Category c) => new()
   {
      Id = c.Id,
      Name = c.Name,
      Description = c.Description
   };
   
}