using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Display(Name = "Dodatki")]
        public string Additions { get; set; }
        public enum EAdditions { NA = 0, Ziemniaki = 1, Frytki = 2, Ryż = 3}

        [Display(Name = "Obraz")]
        public string Image { get; set; }

        [Display(Name = "Kategoria")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Display(Name = "Podkategoria")]
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }

        [Display(Name = "Cena")]
        [DataType(DataType.Currency)]
        //[Range(1, int.MaxValue, ErrorMessage = " Cena powinna być wyższa niż {1} zł")]
        public decimal Price { get; set; }


    }
}
