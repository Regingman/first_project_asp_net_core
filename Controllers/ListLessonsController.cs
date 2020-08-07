using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;

namespace WebApplication3.Controllers
{
    public class ListLessonsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ListLessonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ListLessons
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ListLessons.Include(l => l.Groups).Include(l => l.Lessons).Include(l => l.Teachers);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ListLessons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listLesson = await _context.ListLessons
                .Include(l => l.Groups)
                .Include(l => l.Lessons)
                .Include(l => l.Teachers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listLesson == null)
            {
                return NotFound();
            }

            return View(listLesson);
        }

        // GET: ListLessons/Create
        public IActionResult Create()
        {
            ViewData["Id_group"] = new SelectList(_context.Groups, "Id", "Id");
            ViewData["Id_lesson"] = new SelectList(_context.Lessons, "Id", "Id");
            ViewData["Id_teacher"] = new SelectList(_context.Teachers, "Id", "Id");
            return View();
        }

        // POST: ListLessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NumberLesson,DayOfWeek,Id_group,Id_teacher,Id_lesson")] ListLesson listLesson)
        {
            if (ModelState.IsValid)
            {
                _context.Add(listLesson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id_group"] = new SelectList(_context.Groups, "Id", "Id", listLesson.Id_group);
            ViewData["Id_lesson"] = new SelectList(_context.Lessons, "Id", "Id", listLesson.Id_lesson);
            ViewData["Id_teacher"] = new SelectList(_context.Teachers, "Id", "Id", listLesson.Id_teacher);
            return View(listLesson);
        }

        // GET: ListLessons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listLesson = await _context.ListLessons.FindAsync(id);
            if (listLesson == null)
            {
                return NotFound();
            }
            ViewData["Id_group"] = new SelectList(_context.Groups, "Id", "Id", listLesson.Id_group);
            ViewData["Id_lesson"] = new SelectList(_context.Lessons, "Id", "Id", listLesson.Id_lesson);
            ViewData["Id_teacher"] = new SelectList(_context.Teachers, "Id", "Id", listLesson.Id_teacher);
            return View(listLesson);
        }

        // POST: ListLessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumberLesson,DayOfWeek,Id_group,Id_teacher,Id_lesson")] ListLesson listLesson)
        {
            if (id != listLesson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(listLesson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListLessonExists(listLesson.Id))
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
            ViewData["Id_group"] = new SelectList(_context.Groups, "Id", "Id", listLesson.Id_group);
            ViewData["Id_lesson"] = new SelectList(_context.Lessons, "Id", "Id", listLesson.Id_lesson);
            ViewData["Id_teacher"] = new SelectList(_context.Teachers, "Id", "Id", listLesson.Id_teacher);
            return View(listLesson);
        }

        // GET: ListLessons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listLesson = await _context.ListLessons
                .Include(l => l.Groups)
                .Include(l => l.Lessons)
                .Include(l => l.Teachers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listLesson == null)
            {
                return NotFound();
            }

            return View(listLesson);
        }

        // POST: ListLessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var listLesson = await _context.ListLessons.FindAsync(id);
            _context.ListLessons.Remove(listLesson);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ListLessonExists(int id)
        {
            return _context.ListLessons.Any(e => e.Id == id);
        }
    }
}
