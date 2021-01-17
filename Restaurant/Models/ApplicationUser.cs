using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Imię")]
        public string Name { get; set; }
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }
        [Display(Name = "Adres")]
        public string StreetAddress { get; set; }
        [Display(Name = "Miasto")]
        public string City { get; set; }
        [Display(Name = "Kod Pocztowy")]
        public string PostalCode { get; set; }
        [Display(Name="Imię i Nazwisko")]
        public string FullName
        {
            get { return $"{Name} {LastName}"; }
        }
        

    }
}
