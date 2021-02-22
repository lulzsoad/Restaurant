using Restaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Utility
{
    public static class StaticDetail
    {
        public const string NoImage = "no_image.jpg";
        public const string ManagerUser = "Menadżer";
        public const string KitchenUser = "Kuchnia";
        public const string FrontDeskUser = "Obsługa";
        public const string CustomerEndUser = "Klient";

        public const string ssShoppingCartCount = "ssCartCount";
		public const string ssCouponCode = "ssCouponCode";

		public const string StatusSubmitted = "Wysłane";
		public const string StatusInProccess = "Jest przygotowywane";
		public const string StatusReady = "Gotowe do odbioru";
		public const string StatusCompleted = "Zakończone";
		public const string StatusCancelled = "Anulowane";

		public const string PaymentStatusPending = "Oczekuje";
		public const string PaymentStatusApproved = "Zatwierdzona";
		public const string PaymentStatusRejected = "Odrzucona";


		public static string ConvertToRawHtml(string source)
		{
			char[] array = new char[source.Length];
			int arrayIndex = 0;
			bool inside = false;

			for (int i = 0; i < source.Length; i++)
			{
				char let = source[i];
				if (let == '<')
				{
					inside = true;
					continue;
				}
				if (let == '>')
				{
					inside = false;
					continue;
				}
				if (!inside)
				{
					array[arrayIndex] = let;
					arrayIndex++;
				}
			}
			return new string(array, 0, arrayIndex);
		}

		public static double DiscountedPrice(Coupon couponFromDb, double originalOrderTotal)
        {
			if(couponFromDb == null)
            {
				return originalOrderTotal;
            }
			else
            {
				if(couponFromDb.MinimumAmount > originalOrderTotal)
                {
					return originalOrderTotal;
                }
				else
                {
					// EVERYTHING IS VALID
					if(Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Wartościowy)
                    {
						return Math.Round(originalOrderTotal - couponFromDb.Discount, 2);
                    }
					
					if(Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Procentowy)
                    {
							return Math.Round(originalOrderTotal - (originalOrderTotal * couponFromDb.Discount / 100), 2);
					}
                    
                }
            }
			return originalOrderTotal;
        }
	}
}
