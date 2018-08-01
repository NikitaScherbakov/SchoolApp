using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using SchoolApp.DAL;
using SchoolApp.Models;

namespace SchoolApp.Controllers
{
    public class StudentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, string option, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var students = db.Students
                            .Select(s => s);

            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));

            if (!string.IsNullOrEmpty(searchString))
            {
                if (option == "Gender")
                {
                    var gender = (Gender)Enum.Parse(typeof(Gender), searchString);

                    students = students
                        .Where(s => s.Gender == gender);
                }
            }

            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.ClassDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.ClassDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(students.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return PartialView(student);
        }

        public ActionResult Create()
        {
            PopulateGenderDropDownList();
            PopulateStarostaDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstName,ClassDate,Gender,Starosta")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }            
            catch (RetryLimitExceededException ex)
            {
                ModelState.AddModelError("Unable to save changes while creating student", ex.Message);
            }

            PopulateGenderDropDownList(student);
            PopulateStarostaDropDownList(student);
            return View(student);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            PopulateGenderDropDownList(student.Gender);
            PopulateStarostaDropDownList(student.Starosta);
            return PartialView(student);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var studentToUpdate = db.Students.Find(id);
            if (TryUpdateModel(studentToUpdate, "",
                new string[] { "LastName", "FirstName", "ClassDate", "Gender", "Starosta" }))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException ex)
                {
                    ModelState.AddModelError("Unable to save changes while editing student", ex.Message);
                }
            }

            PopulateGenderDropDownList(studentToUpdate.Gender);
            PopulateStarostaDropDownList(studentToUpdate.Starosta);
            return View(studentToUpdate);
        }

        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete student failed";
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return PartialView(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Student student = db.Students.Find(id);
                db.Students.Remove(student);
                db.SaveChanges();
            }
            catch (RetryLimitExceededException ex)
            {
                ModelState.AddModelError("Unable to save changes while deleting student", ex.Message);
                return RedirectToAction("Delete", new { id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        private void PopulateStarostaDropDownList(object selected = null)
        {
            ViewBag.Starosta = new SelectList(Enum.GetNames(typeof(Master)), selectedValue: selected);
        }

        private void PopulateGenderDropDownList(object selected = null)
        {
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)), selectedValue: selected);
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
