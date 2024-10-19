using System;
using System.Security;

namespace FileEncryptor
{
    public static class PasswordReader
    {
        public static SecureString ReadPassword()
        {
            SecureString password = new SecureString();
            ConsoleKeyInfo key;

            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.RemoveAt(password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    password.AppendChar(key.KeyChar);
                    Console.Write("*");
                }
            }

            Console.WriteLine();
            return password;
        }
    }
}