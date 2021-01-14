using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;

namespace Restaurant.Areas.Admin.Controllers
{
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
    }
}
