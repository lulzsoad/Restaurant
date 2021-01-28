using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Utility;

namespace Restaurant.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetail.ManagerUser)]
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> Index()
        {
            var claimsIndetity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIndetity.FindFirst(ClaimTypes.NameIdentifier); // FTEHCES LOGGED-IN USER



            return View(await _db.ApplicationUser.Where(u=>u.Id != claim.Value).ToListAsync()); //GETS EVERY USER EXCEPT OF LOGGED IN ONE
        }

        public async Task<IActionResult> Lock(string id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);

            if(applicationUser == null)
            {
                return NotFound();
            }

            applicationUser.LockoutEnd = DateTime.Now.AddYears(1000);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Unlock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            applicationUser.LockoutEnd = DateTime.Now;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
