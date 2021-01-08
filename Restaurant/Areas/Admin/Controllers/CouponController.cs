using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Models;

namespace Restaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public Coupon Coupon { get; set; }
        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupon.ToListAsync());
        }

        // (GET - CREATE)
        public IActionResult Create()
        {
            return View();
        }

        // ( CREATE - POST )
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if(ModelState.IsValid)
            {

                // SAVING PICTURE DIRECTLY TO DATABASE
                var files = HttpContext.Request.Form.Files;
                if(files.Count>0)
                {
                    byte[] p1 = null;
                    using(var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray(); // Transforms picture into string
                        }
                    }
                    Coupon.Picture = p1;
                }
                _db.Coupon.Add(Coupon);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(Coupon);
        }

        // (GET - EDIT)

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            Coupon = await _db.Coupon.Where(m => m.Id == id).FirstOrDefaultAsync();

            if (Coupon == null)
            {
                return NotFound();
            }

            return View(Coupon);
        }

        // (POST - EDIT)

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int? id)
        {
            if (ModelState.IsValid)
            {
                var couponFromDb = await _db.Coupon.FindAsync(Coupon.Id);

                // SAVING PICTURE DIRECTLY TO DATABASE
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray(); // Transforms picture into string
                        }
                    }
                    couponFromDb.Picture = p1;
                }

                couponFromDb.Name = Coupon.Name;
                couponFromDb.IsActive = Coupon.IsActive;
                couponFromDb.MinimumAmount = Coupon.MinimumAmount;
                couponFromDb.Discount = Coupon.Discount;
                couponFromDb.CouponType = Coupon.CouponType;
                
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(Coupon);
        }

        // (GET - DETAILS)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Coupon = await _db.Coupon.Where(m => m.Id == id).FirstOrDefaultAsync();

            if (Coupon == null)
            {
                return NotFound();
            }

            return View(Coupon);
        }

        // (GET - DELETE)

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Coupon = await _db.Coupon.Where(m => m.Id == id).FirstOrDefaultAsync();

            if (Coupon == null)
            {
                return NotFound();
            }

            return View(Coupon);
        }

        // (POST - DELETE)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var coupon = await _db.Coupon.FindAsync(Coupon.Id);

            if(coupon == null)
            {
                return NotFound();
            }

            _db.Coupon.Remove(coupon);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
