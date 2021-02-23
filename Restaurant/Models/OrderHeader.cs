using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Display(Name = "Data Zamówienia")]
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal OrderTotalOriginal { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Suma")]
        public decimal OrderTotal { get; set; }

        [Required]
        [Display(Name = "Godzina odbioru")]
        public DateTime PickUpTime { get; set; }

        [Required]
        [NotMapped]
        [Display(Name = "Data odbioru")]
        public DateTime PickUpDate { get; set; }

        [Display(Name = "Kod promocyjny")]
        public string CouponCode { get; set; }
        public decimal CouponCodeDiscount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }

        [Display(Name = "Uwagi")]
        public string Comments { get; set; }

        [Display(Name = "Imię")]
        public string PickUpName { get; set; }

        [Display(Name = "Numer telefonu")]
        public string PhoneNumber { get; set; }

        public string TransactionId { get; set; }

    }
}
