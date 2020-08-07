using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;

namespace WebApplication3.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListLessonsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ListLessonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ListLessons
        [HttpGet]
        public IEnumerable<ListLesson> GetListLessons()
        {
            return _context.ListLessons;
        }

        // GET: api/ListLessons/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetListLesson([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var listLesson = await _context.ListLessons.FindAsync(id);

            if (listLesson == null)
            {
                return NotFound();
            }

            return Ok(listLesson);
        }

        // PUT: api/ListLessons/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutListLesson([FromRoute] int id, [FromBody] ListLesson listLesson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != listLesson.Id)
            {
                return BadRequest();
            }

            _context.Entry(listLesson).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListLessonExists(id))
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

        // POST: api/ListLessons
        [HttpPost]
        public async Task<IActionResult> PostListLesson([FromBody] ListLesson listLesson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ListLessons.Add(listLesson);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetListLesson", new { id = listLesson.Id }, listLesson);
        }

        // DELETE: api/ListLessons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteListLesson([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var listLesson = await _context.ListLessons.FindAsync(id);
            if (listLesson == null)
            {
                return NotFound();
            }

            _context.ListLessons.Remove(listLesson);
            await _context.SaveChangesAsync();

            return Ok(listLesson);
        }

        private bool ListLessonExists(int id)
        {
            return _context.ListLessons.Any(e => e.Id == id);
        }
    }
}