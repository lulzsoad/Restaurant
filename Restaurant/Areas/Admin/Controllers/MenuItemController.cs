using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Models.ViewModels;
using Restaurant.Utility;

namespace Restaurant.Areas.Admin.Controllers
{
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

            if(ModelState.IsValid)
            {
                _db.MenuItem.Add(MenuItemVM.MenuItem);
                await _db.SaveChangesAsync();

                // IMAGE SAVING SECTION

                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

                if(files.Count > 0)
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
            else
            {
                return View(MenuItemVM);
            }
        }

    }
}
