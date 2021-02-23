using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Models;
using Restaurant.Models.ViewModels;
using Restaurant.Utility;
using Stripe;

namespace Restaurant.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public OrderDetailsCartViewModel DetailCartVM { get; set; }

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            DetailCartVM = new OrderDetailsCartViewModel()
            {
                OrderHeader = new OrderHeader()
            };

            DetailCartVM.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);

            if(cart != null)
            {
                DetailCartVM.ListCart = cart.ToList();
            }

            foreach(var list in DetailCartVM.ListCart)
            {
                list.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(m => m.Id == list.MenuItemId);
                DetailCartVM.OrderHeader.OrderTotal += (list.MenuItem.Price * list.Count);
                list.MenuItem.Description = StaticDetail.ConvertToRawHtml(list.MenuItem.Description);
                if(list.MenuItem.Description.Length > 100)
                {
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 99) + "...";
                }
            }

            DetailCartVM.OrderHeader.OrderTotalOriginal = DetailCartVM.OrderHeader.OrderTotal;

            if(HttpContext.Session.GetString(StaticDetail.ssCouponCode) != null)
            {
                DetailCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(StaticDetail.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == DetailCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                DetailCartVM.OrderHeader.OrderTotal = StaticDetail.DiscountedPrice(couponFromDb, DetailCartVM.OrderHeader.OrderTotalOriginal);
            }

            return View(DetailCartVM);
        }

        public IActionResult AddCoupon()
        {
            if(DetailCartVM.OrderHeader.CouponCode == null)
            {
                DetailCartVM.OrderHeader.CouponCode = "";
            }

            HttpContext.Session.SetString(StaticDetail.ssCouponCode, DetailCartVM.OrderHeader.CouponCode);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.SetString(StaticDetail.ssCouponCode, string.Empty);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(c => c.Id == cartId);
            cart.Count += 1;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(c => c.Id == cartId);

            if(cart.Count == 1)
            {
                _db.ShoppingCart.Remove(cart);
                await _db.SaveChangesAsync();

                var cnt = _db.ShoppingCart.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(StaticDetail.ssShoppingCartCount, cnt);
            }
            else
            {
                cart.Count -= 1;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(c => c.Id == cartId);

            _db.ShoppingCart.Remove(cart);
            await _db.SaveChangesAsync();

            var cnt = _db.ShoppingCart.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(StaticDetail.ssShoppingCartCount, cnt);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Summary()
        {
            DetailCartVM = new OrderDetailsCartViewModel()
            {
                OrderHeader = new OrderHeader()
            };

            DetailCartVM.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser appUser = await _db.ApplicationUser.Where(c => c.Id == claim.Value).FirstOrDefaultAsync();

            var cart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                DetailCartVM.ListCart = cart.ToList();
            }

            foreach (var list in DetailCartVM.ListCart)
            {
                list.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(m => m.Id == list.MenuItemId);
                DetailCartVM.OrderHeader.OrderTotal += (list.MenuItem.Price * list.Count);
            }

            DetailCartVM.OrderHeader.OrderTotalOriginal = DetailCartVM.OrderHeader.OrderTotal;
            DetailCartVM.OrderHeader.PickUpName = appUser.Name;
            DetailCartVM.OrderHeader.PhoneNumber = appUser.PhoneNumber;
            DetailCartVM.OrderHeader.PickUpTime = DateTime.Now;

            if (HttpContext.Session.GetString(StaticDetail.ssCouponCode) != null)
            {
                DetailCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(StaticDetail.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == DetailCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                DetailCartVM.OrderHeader.OrderTotal = StaticDetail.DiscountedPrice(couponFromDb, DetailCartVM.OrderHeader.OrderTotalOriginal);
            }

            return View(DetailCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPOST(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            DetailCartVM.ListCart = await _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value).ToListAsync();

            DetailCartVM.OrderHeader.PaymentStatus = StaticDetail.PaymentStatusPending;
            DetailCartVM.OrderHeader.OrderDate = DateTime.Now;
            DetailCartVM.OrderHeader.UserId = claim.Value;
            DetailCartVM.OrderHeader.Status = StaticDetail.PaymentStatusPending;
            DetailCartVM.OrderHeader.PickUpDate = Convert.ToDateTime(DetailCartVM.OrderHeader.PickUpDate.ToShortDateString() + " " + DetailCartVM.OrderHeader.PickUpTime.ToShortTimeString());

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _db.OrderHeader.Add(DetailCartVM.OrderHeader);
            await _db.SaveChangesAsync();

            DetailCartVM.OrderHeader.OrderTotalOriginal = 0; 

            foreach (var item in DetailCartVM.ListCart)
            {
                item.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(m => m.Id == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails()
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = DetailCartVM.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                DetailCartVM.OrderHeader.OrderTotalOriginal += orderDetails.Count * orderDetails.Price;
                _db.OrderDetails.Add(orderDetails);

            }

            if (HttpContext.Session.GetString(StaticDetail.ssCouponCode) != null)
            {
                DetailCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(StaticDetail.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == DetailCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                DetailCartVM.OrderHeader.OrderTotal = StaticDetail.DiscountedPrice(couponFromDb, DetailCartVM.OrderHeader.OrderTotalOriginal);
            }
            else
            {
                DetailCartVM.OrderHeader.OrderTotal = DetailCartVM.OrderHeader.OrderTotalOriginal;
            }

            DetailCartVM.OrderHeader.CouponCodeDiscount = DetailCartVM.OrderHeader.OrderTotalOriginal - DetailCartVM.OrderHeader.OrderTotal;
            await _db.SaveChangesAsync();

            _db.ShoppingCart.RemoveRange(DetailCartVM.ListCart);
            HttpContext.Session.SetInt32(StaticDetail.ssShoppingCartCount, 0);
            await _db.SaveChangesAsync();

            var options = new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(DetailCartVM.OrderHeader.OrderTotal * 100),
                Currency = "pln",
                Description = "ID Zamówienia : " + DetailCartVM.OrderHeader.Id,
                SourceId = stripeToken
            };

            var service = new ChargeService();
            Charge charge = service.Create(options);

            if(charge.BalanceTransactionId == null)
            {
                DetailCartVM.OrderHeader.PaymentStatus = StaticDetail.PaymentStatusRejected;
            }
            else
            {
                DetailCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;
            }

            if(charge.Status.ToLower() == "succeeded")
            {
                DetailCartVM.OrderHeader.PaymentStatus = StaticDetail.PaymentStatusApproved;
                DetailCartVM.OrderHeader.Status = StaticDetail.StatusSubmitted;
            }
            else
            {
                DetailCartVM.OrderHeader.PaymentStatus = StaticDetail.PaymentStatusRejected;
            }

            await _db.SaveChangesAsync();

            //return RedirectToAction("Index", "Home");
            return RedirectToAction("Confirm", "Order", new { id = DetailCartVM.OrderHeader.Id });
        }

        
    }
}
