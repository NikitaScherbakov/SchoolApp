using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using SchoolApp.DAL;
using SchoolApp.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace SchoolApp.Controllers
{
    public class DepartmentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        public async Task<ActionResult> Index()
        {
            var departments = db.Departments.Include(d => d.Administrator);
            return View(await departments.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return PartialView(department);
        }

        public ActionResult Create()
        {
            ViewBag.TeacherID = new SelectList(db.Teachers, "ID", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DepartmentID,Name,Budget,StartDate,TeacherID")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.TeacherID = new SelectList(db.Teachers, "ID", "FullName", department.TeacherID);
            return View(department);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            ViewBag.TeacherID = new SelectList(db.Teachers, "ID", "FullName", department.TeacherID);
            return PartialView(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, byte[] rowVersion)
        {
            string[] fieldsToBind = new string[] { "Name", "Budget", "StartDate", "TeacherID", "RowVersion" };

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var departmentToUpdate = await db.Departments.FindAsync(id);
            if (departmentToUpdate == null)
            {
                Department deletedDepartment = new Department();
                TryUpdateModel(deletedDepartment, fieldsToBind);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The department was deleted by another user.");
                ViewBag.TeacherID = new SelectList(db.Teachers, "ID", "FullName", deletedDepartment.TeacherID);
                return View(deletedDepartment);
            }

            if (TryUpdateModel(departmentToUpdate, fieldsToBind))
            {
                try
                {
                    db.Entry(departmentToUpdate).OriginalValues["RowVersion"] = rowVersion;
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var clientValues = (Department)entry.Entity;
                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The department was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Department)databaseEntry.ToObject();

                        if (databaseValues.Name != clientValues.Name)
                            ModelState.AddModelError("Name", "Current value: "
                                + databaseValues.Name);
                        if (databaseValues.Budget != clientValues.Budget)
                            ModelState.AddModelError("Budget", "Current value: "
                                + string.Format("{0:c}", databaseValues.Budget));
                        if (databaseValues.StartDate != clientValues.StartDate)
                            ModelState.AddModelError("StartDate", "Current value: "
                                + string.Format("{0:d}", databaseValues.StartDate));
                        if (databaseValues.TeacherID != clientValues.TeacherID)
                            ModelState.AddModelError("TeacherID", "Current value: "
                                + db.Teachers.Find(databaseValues.TeacherID).FullName);
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                            + "was modified by another user after you got the original value. The "
                            + "edit operation was canceled and the current values in the database "
                            + "have been displayed. If you still want to edit this record, click "
                            + "the Save button again. Otherwise click the Back to List hyperlink.");
                        departmentToUpdate.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException ex)
                {
                    ModelState.AddModelError("Unable to save changes while creating course", ex.Message);
                }
            }
            ViewBag.TeacherID = new SelectList(db.Teachers, "ID", "FullName", departmentToUpdate.TeacherID);
            return View(departmentToUpdate);
        }

        public async Task<ActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("Index");
                }
                return HttpNotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return PartialView(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Department department)
        {
            try
            {
                db.Entry(department).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = department.DepartmentID });
            }
            catch (RetryLimitExceededException ex)
            {
                ModelState.AddModelError("Unable to save changes while creating course", ex.Message);
                return View(department);
            }
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
