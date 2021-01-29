using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }

        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        [NotMapped]             // It means -Doesn't add field to database
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int MenuItemId { get; set; }
        [NotMapped]             // It means -Doesn't add field to database
        [ForeignKey("MenuItemId")]
        public virtual ApplicationUser MenuItem { get; set; }

        [Display(Name = "Ilość")]
        [Range(1, int.MaxValue, ErrorMessage = "Proszę wprowadzić liczbę większą niż {1}")]
        public int Count { get; set; }
    }
}
