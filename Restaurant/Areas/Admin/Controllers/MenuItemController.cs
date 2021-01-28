using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Models.ViewModels;
using Restaurant.Utility;

namespace Restaurant.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetail.ManagerUser)]
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        // Item loaded by default to functions
        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;

            MenuItemVM = new MenuItemViewModel()
            {
                Category = _db.Category,
                MenuItem = new Models.MenuItem() 
            };
        }

        public async Task<IActionResult> Index()
        {
            var menuItems = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(); // Fetches Menu Items in a list including Category and Subcategory fields
            return View(menuItems);
        }

        // (GET - CREATE)
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        // (POST - CREATE)
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if(MenuItemVM.MenuItem.Price == 0)
            {
                MenuItemVM.MenuItem.Price = 1.99;
            }

            if (!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }

            _db.MenuItem.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();

            // IMAGE SAVING SECTION

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            if (files.Count > 0)
            {
                // FILE HAS BEEN UPLOADED

                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                // SETS FILE'S NAME BY ID OF MENUITEM WITH ACTUAL EXTENSION
                using (var fileStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                menuItemFromDb.Image = $@"\images\{MenuItemVM.MenuItem.Id + extension}";
            }
            else
            {
                // NO FILE WAS UPLOADED, SO USE DEFAULT

                var uploads = Path.Combine(webRootPath, $@"images\{StaticDetail.NoImage}");
                System.IO.File.Copy(uploads, $@"{webRootPath}\images\{MenuItemVM.MenuItem.Id}.jpg");
                menuItemFromDb.Image = $@"\images\{MenuItemVM.MenuItem.Id}.jpg";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // (GET - EDIT)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        // (POST - EDIT)
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (ModelState.IsValid)
            {
                // IMAGE SAVING SECTION

                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

                if (files.Count > 0)
                {
                    // NEW FILE HAS BEEN UPLOADED

                    var uploads = Path.Combine(webRootPath, "images");
                    var extension_new = Path.GetExtension(files[0].FileName);

                    // DELETING ORIGINAL FILE

                    if (menuItemFromDb.Image == null)
                    {
                        using (var fileStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_new), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                    }
                    else
                    {
                        var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }


                    // UPLOADS NEW FILE
                    using (var fileStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    menuItemFromDb.Image = $@"\images\{MenuItemVM.MenuItem.Id + extension_new}";
                }

                menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
                menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
                menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
                menuItemFromDb.Additions = MenuItemVM.MenuItem.Additions;
                menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
                menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
                return View(MenuItemVM);
            }
        }

        // (GET - Details)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        // (GET - Delete)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        // (GET - Delete)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            if (menuItemFromDb.Image != null)
            {
                var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }


            _db.MenuItem.Remove(menuItemFromDb);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
