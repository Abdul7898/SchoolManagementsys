using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly AppDbContext _context;

        public EnrollmentsController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: Enrollments
        // =========================
        public async Task<IActionResult> Index()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();

            return View(enrollments);
        }

        // =========================
        // GET: Enrollments/Details/5
        // =========================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enrollment == null)
                return NotFound();

            return View(enrollment);
        }

        // =========================
        // GET: Enrollments/Create
        // =========================
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // =========================
        // POST: Enrollments/Create
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int StudentId, int CourseId)
        {
            // 🔴 HARD CHECK
            if (StudentId == 0 || CourseId == 0)
            {
                LoadDropdowns();
                ViewBag.Error = "Please select both Student and Course";
                return View();
            }

            var enrollment = new Enrollment
            {
                StudentId = StudentId,
                CourseId = CourseId
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // =========================
        // GET: Enrollments/Edit/5
        // =========================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
                return NotFound();

            LoadDropdowns(enrollment.StudentId, enrollment.CourseId);
            return View(enrollment);
        }

        // =========================
        // POST: Enrollments/Edit/5
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Enrollment enrollment)
        {
            if (id != enrollment.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns(enrollment.StudentId, enrollment.CourseId);
            return View(enrollment);
        }

        // =========================
        // GET: Enrollments/Delete/5
        // =========================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enrollment == null)
                return NotFound();

            return View(enrollment);
        }

        // =========================
        // POST: Enrollments/Delete/5
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);

            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // Helper Methods
        // =========================
        private void LoadDropdowns(int? studentId = null, int? courseId = null)
        {
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Name", studentId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", courseId);
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.Id == id);
        }
    }
}
