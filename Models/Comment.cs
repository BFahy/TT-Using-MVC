using System;
using System.ComponentModel.DataAnnotations;

namespace TypicalTools.Models
{
    public class Comment
    {
        public int id { get; set; }
        [Required]
        [Display(Name = "Comment")]
        [MaxLength(50, ErrorMessage ="Message exceeds maximum length")]
        public string text { get; set; }
        [Display(Name = "Product Code")]
        public int product_id { get; set; }
        public string session_id { get; set; }
        [Display(Name = "Date Created")]
        public DateTime created_date { get; set; }

    }
}
