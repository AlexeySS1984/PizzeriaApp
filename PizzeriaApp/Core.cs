using System.Security.Cryptography;
using System.Text;

namespace PizzeriaApp
{
    public static class Core
    {
        // Именно тот класс Core, который требуется по заданию.
        public static PizzeriaDBEntities Context = new PizzeriaDBEntities();

        public static Users CurrentUser { get; set; }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                    builder.Append(hash[i].ToString("X2"));

                return builder.ToString();
            }
        }
    }
}
