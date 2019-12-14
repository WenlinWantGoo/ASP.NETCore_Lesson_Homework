using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversityDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContosoUniversityDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        public CoursesController(ContosouniversityContext context)
        {
            _context = context;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourse()
        {
            return await _context.Course.Where(x => !x.IsDeleted).ToListAsync();
        }

        [HttpGet]
        [Route("course-bug")]
        public ActionResult<int> GetExceptionTest()
        {
            var a = 0;
            var b = 1 / a;
            return b;
        }

        [HttpGet]
        [Route("course-students")]
        public ActionResult<IEnumerable<VwCourseStudents>> GetCourseStudents()
        {
            return _context.VwCourseStudents
                .FromSqlInterpolated($"SELECT * FROM dbo.vwCourseStudents")
                .ToList();
        }

        [HttpGet]
        [Route("course-student-count")]
        public ActionResult<IEnumerable<VwCourseStudentCount>> GetCourseStudentsCount()
        {
            return _context.VwCourseStudentCount
                .FromSqlInterpolated($"SELECT * FROM dbo.VwCourseStudentCount")
                .ToList();
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _context.Course.Where(x=>x.CourseId.Equals(id) && !x.IsDeleted).SingleOrDefaultAsync();

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchCourse(int id, CourseViewModel course)
        {
            var result = this._context.Course.Find(id);

            if (!await TryUpdateModelAsync(result))
            {
                return BadRequest();
            }

            result.Credits = course.Credits;
            _context.Entry(result).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
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


        // PUT: api/Courses/5
        [HttpPut("{id}")]
       public async Task<IActionResult> PutCourse(int id, Course course)
        {
            if (id != course.CourseId)
            {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
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

        // POST: api/Courses
        [HttpPost]
        public async Task<ActionResult<Course>> PostCourse(Course course)
        {
            _context.Course.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourse), new {id = course.CourseId}, course);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Course>> DeleteCourse(int id)
        {
            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            //_context.Course.Remove(course);
            course.IsDeleted = true;
            await _context.SaveChangesAsync();

            return course;
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.CourseId == id);
        }
    }

    public class CourseViewModel
    {
        [Range(0,10)]
        public int Credits { get; set; }

    }
}