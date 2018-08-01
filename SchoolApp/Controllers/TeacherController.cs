using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SchoolApp.DAL;
using SchoolApp.Models;
using SchoolApp.ViewModels;

namespace SchoolApp.Controllers
{
    public class TeacherController : Controller
    {
        private SchoolContext db = new SchoolContext();

        public ActionResult Index(int? id, int? courseID)
        {
            var viewModel = new TeacherIndexData
            {
                Teachers = db.Teachers
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses.Select(c => c.Department))
                .OrderBy(i => i.LastName)
            };

            if (id != null)
            {
                ViewBag.TeacherID = id.Value;
                viewModel.Courses = viewModel.Teachers
                                            .Where(i => i.ID == id.Value).Single().Courses;
            }

            if (courseID != null)
            {
                ViewBag.CourseID = courseID.Value;
                viewModel.Classs = viewModel.Courses
                                            .Where(c => c.CourseID == courseID).Single().Classs;
            }

            return View(viewModel);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var Teacher = db.Teachers.Find(id);
            if (Teacher == null)
            {
                return HttpNotFound();
            }
            return PartialView(Teacher);
        }

        public ActionResult Create()
        {
            var teacher = new Teacher
            {
                Courses = new List<Course>()
            };
            PopulateAssignedCourseData(teacher);
            PopulateGenderDropDownList(teacher);
            PopulateDirectorDropDownList(teacher);
            PopulateHeadTeacherDropDownList(teacher);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,LastName,FirstName,HireDate,Gender,Director,HeadTeacher")] Teacher teacher, string[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                teacher.Courses = new List<Course>();
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = db.Courses.Find(int.Parse(course));
                    teacher.Courses.Add(courseToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                db.Teachers.Add(teacher);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateAssignedCourseData(teacher);
            PopulateGenderDropDownList(teacher);
            PopulateDirectorDropDownList(teacher);
            PopulateHeadTeacherDropDownList(teacher);
            return View(teacher);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var teacher = db.Teachers
                                        .Include(i => i.OfficeAssignment)
                                        .Include(i => i.Courses)
                                        .Where(i => i.ID == id)
                                        .Single();
            PopulateAssignedCourseData(teacher);
            if (teacher == null)
            {
                return HttpNotFound();
            }

            PopulateGenderDropDownList(teacher);
            PopulateDirectorDropDownList(teacher);
            PopulateHeadTeacherDropDownList(teacher);
            return PartialView(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedCourses)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var teacherToUpdate = db.Teachers
                                            .Include(i => i.OfficeAssignment)
                                            .Include(i => i.Courses)
                                            .Where(i => i.ID == id)
                                            .Single();

            if (TryUpdateModel(teacherToUpdate, "",
                new string[] { "LastName", "FirstName", "HireDate", "OfficeAssignment", "Gender", "Director", "HeadTeacher" }))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(teacherToUpdate.OfficeAssignment.Location))
                    {
                        teacherToUpdate.OfficeAssignment = null;
                    }

                    UpdateTeacherCourses(selectedCourses, teacherToUpdate);

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                catch (RetryLimitExceededException ex)
                {
                    ModelState.AddModelError("Unable to save changes while editing Teacher", ex.Message);
                }
            }

            PopulateAssignedCourseData(teacherToUpdate);
            PopulateGenderDropDownList(teacherToUpdate);
            PopulateDirectorDropDownList(teacherToUpdate);
            PopulateHeadTeacherDropDownList(teacherToUpdate);
            return View(teacherToUpdate);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var Teacher = db.Teachers.Find(id);
            if (Teacher == null)
            {
                return HttpNotFound();
            }
            return PartialView(Teacher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var Teacher = db.Teachers
                                    .Include(i => i.OfficeAssignment)
                                    .Where(i => i.ID == id)
                                    .Single();

            db.Teachers.Remove(Teacher);

            var department = db.Departments
                                    .Where(d => d.TeacherID == id)
                                    .SingleOrDefault();
            if (department != null)
            {
                department.TeacherID = null;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void PopulateAssignedCourseData(Teacher Teacher)
        {
            var allCourses = db.Courses;
            var TeacherCourses = new HashSet<int>(Teacher.Courses.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = TeacherCourses.Contains(course.CourseID)
                });
            }
            ViewBag.Courses = viewModel;
        }

        private void UpdateTeacherCourses(string[] selectedCourses, Teacher TeacherToUpdate)
        {
            if (selectedCourses == null)
            {
                TeacherToUpdate.Courses = new List<Course>();
                return;
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var TeacherCourses = new HashSet<int>
                (TeacherToUpdate.Courses.Select(c => c.CourseID));
            foreach (var course in db.Courses)
            {
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    if (!TeacherCourses.Contains(course.CourseID))
                    {
                        TeacherToUpdate.Courses.Add(course);
                    }
                }
                else
                {
                    if (TeacherCourses.Contains(course.CourseID))
                    {
                        TeacherToUpdate.Courses.Remove(course);
                    }
                }
            }
        }

        private void PopulateGenderDropDownList(object selected = null)
        {
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)), selectedValue: selected);
        }

        private void PopulateDirectorDropDownList(object selected = null)
        {
            ViewBag.Director = new SelectList(Enum.GetNames(typeof(Master)), selectedValue: selected);
        }

        private void PopulateHeadTeacherDropDownList(object selected = null)
        {
            ViewBag.HeadTeacher = new SelectList(Enum.GetNames(typeof(Master)), selectedValue: selected);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
