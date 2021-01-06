﻿using System;
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
    }
}
