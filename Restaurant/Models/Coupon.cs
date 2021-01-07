using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Nazwa")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Typ Kuponu")]
        public string CouponType { get; set; }

        public enum ECouponType { Procentowy=0, Wartościowy=1};

        [Display(Name = "Rabat")]
        [Required]
        public double Discount { get; set; }

        [Display(Name = "Minimalna Stawka")]
        [Required]
        public double MinimumAmount { get; set; }

        [Display(Name = "Obrazek")]
        public byte[] Picture { get; set; }

        [Display(Name = "Kupon aktywny?")]
        public bool IsActive { get; set; }
    }
}
