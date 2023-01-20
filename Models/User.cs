using System.ComponentModel.DataAnnotations;

namespace TypicalTools.Models
{
    public class User
    {
        public int id { get; set; }
        [MaxLength(50)]
        [MinLength(4)]
        public string username { get; set; }
        [MaxLength(50)]
        [MinLength(4)]
        public string password { get; set; }

        public string salt { get; set; }
        public string role { get; set; }
    }
}
