using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using SimpleAuthentication03.Models;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.Data.Entity;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleAuthentication03.Controllers
{
    [Authorize]
    public class ToDoController : Controller
    {
        private ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ToDoController(
            UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        }
        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUserAsync();
            //var currentUser = await _userManager.FindByIdAsync(User.GetUserId());
            try
            {
                return View(_db.ToDoes.Where(todo => todo.User.Id == currentUser.Id));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);

                return View();
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ToDo todo)
        {
            var currentUser = await _userManager.FindByIdAsync(User.GetUserId());
            try
            {
                todo.User = currentUser;
                _db.ToDoes.Add(todo);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                return View(todo);
            }
        }

        public IActionResult Edit(int id)
        {
            ToDo todo = _db.ToDoes.FirstOrDefault(d => d.Id == id);
            return View(todo);
        }
        [HttpPost]
        public IActionResult Edit (ToDo todo)
        {
            _db.Entry(todo).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            ToDo todo = _db.ToDoes.FirstOrDefault(d => d.Id == id);
            return View(todo);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            ToDo todo = _db.ToDoes.FirstOrDefault(d => d.Id == id);
            _db.ToDoes.Remove(todo);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
