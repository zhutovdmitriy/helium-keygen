using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

namespace helium_keygen
{
    class Keygen
    {
        private const string productVector = "t3piq7Q5t0WN1zmr";
        private const string productSec = "bc7b0b0a-0dcd-42e6-9601-d99ff87b9bc9";
        private const string productSalt = "aselrias38490a32";
        public string Generate(string name, string email)
        {
            byte[] keyBytes;
            using (AesManaged aesManaged = new AesManaged())
            {
                byte[] vec = Encoding.UTF8.GetBytes(productVector);
                byte[] salt = Encoding.UTF8.GetBytes(productSalt);
                byte[] clearText = Encoding.UTF8.GetBytes(name + "|" + email);
                PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(productSec, salt, "SHA1", 2);
                byte[] key = passwordDeriveBytes.GetBytes(32);
                aesManaged.Mode = CipherMode.CBC;
                using (ICryptoTransform cryptoTransform = aesManaged.CreateEncryptor(key, vec))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(clearText, 0, clearText.Length);
                            cryptoStream.FlushFinalBlock();
                            keyBytes = memoryStream.ToArray();
                        }
                    }
                }
                aesManaged.Clear();
            }
            return Convert.ToBase64String(keyBytes);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Title = "Keygen for Helium 14.x";
            const string title = @"
------------------------------------------------
 Keygen for Helium 14.x
 software url https://www.imploded.com/helium
 by v-i-k-r-a-m (v-i-k-r-a-m@krypdon.dev)
------------------------------------------------
 ";
            Keygen keygen = new Keygen();
            Console.Write(title);
            Console.Write("Name: ");
            var name = Console.ReadLine();
            if (String.IsNullOrEmpty(name))
            {
                name = "abcd";
                Console.WriteLine("Using name  \"abcd\"");
            }
            Console.Write(" Email: ");
            var email = Console.ReadLine();
            if (String.IsNullOrEmpty(email))
            {
                email = "abcd@example.com";
                Console.WriteLine("Using email  \"abcd@example.com\"");
            }
            Console.WriteLine(string.Format(" Key: {0}\n", keygen.Generate(name, email)));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" Block the app in firewall.\n");
            System.Threading.Thread.Sleep(2000);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(" Enter any key to exit.");
            Console.ReadKey();

        }
    }
}
