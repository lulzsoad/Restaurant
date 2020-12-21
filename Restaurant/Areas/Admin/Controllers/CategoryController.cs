using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Models;

namespace Restaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }


        // (GET) Fetches all Category data from database and passes it to the Index View
        public async Task<IActionResult> Index()
        {
            return View(await _db.Category.ToListAsync());
        }

        // (GET - CREATE)
        public IActionResult Create()
        {
            return View();
        }

        //(POST - CREATE)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            // Checks if data in form are valid
            if(ModelState.IsValid)
            {
                _db.Category.Add(category);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // (GET - EDIT)
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
                return NotFound();
            
            var category = await _db.Category.FindAsync(id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        // (GET - POST)
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if(ModelState.IsValid)
            {
                _db.Update(category);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        //(GET - DETAILS)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _db.Category.FindAsync(id);

            if (category == null)
                return NotFound();

            return View(category);
        }


        // (GET - DELETE)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _db.Category.FindAsync(id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var category = await _db.Category.FindAsync(id);

            if (category == null)
                return NotFound();

            _db.Category.Remove(category);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
