using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class PasswordHasher
    {
        public static string ConvertStringToHash(string input)
        {
            using (SHA256 algorithm = SHA256.Create())
            {
                byte[] hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(hash);
            }
        }

        public static string GenerateSalt()
        {
            Random rnd = new Random();
            byte[] bytes = new byte[10];
            rnd.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
