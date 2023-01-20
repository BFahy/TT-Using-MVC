using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TypicalTools.Models
{
    public class Product
    {
        public int id { get; set; }
        
        [Required]
        [MaxLength(50)]
        [Display(Name = "Product Name")]
        public string name { get; set; }
        [Required]
        [Display(Name = "Price")]
        [Range(0, double.MaxValue, ErrorMessage ="Please enter a valid number")]
        public decimal price { get; set; }
        [Display(Name = "Description")]
        public string description { get; set; }
        public DateTime? updated_date { get; set; }
    }
}
