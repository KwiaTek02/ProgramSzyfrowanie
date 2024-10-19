using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace FileEncryptor
{
    public static class Encryptor
    {
        public static void HandleEncrypt(string[] args)
        {
            if (Array.Exists(args, element => element == "--help"))
            {
                HelpPrinter.PrintHelp();
                return;
            }

            string filePath = null;
            string outputPath = null;
            string algorithm = "aes";  // Domyślnie AES
            bool keepOriginal = false;

            // Parsowanie argumentów
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-a":
                        algorithm = args[++i];
                        break;
                    case "-out":
                        outputPath = args[++i];
                        break;
                    case "-b":
                        keepOriginal = true;
                        break;
                    default:
                        filePath = args[i];
                        break;
                }
            }

            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("Nie podano pliku wejściowego. Użyj --help dla pomocy.");
                return;
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                // Domyślna ścieżka wyjściowa, jeśli nie podano `-out`
                outputPath = Path.ChangeExtension(filePath, ".enc");
            }

            Console.Write("Podaj hasło: ");
            SecureString password = PasswordReader.ReadPassword();

            byte[] fileContent = File.ReadAllBytes(filePath);
            byte[] encryptedContent = null;

            if (algorithm.ToLower() == "aes")
            {
                encryptedContent = EncryptAES(fileContent, SecureStringToString(password));
            }
            else if (algorithm.ToLower() == "des")
            {
                encryptedContent = EncryptDES(fileContent, SecureStringToString(password));
            }
            else
            {
                Console.WriteLine("Nieznany algorytm. Dostępne algorytmy to 'aes' i 'des'.");
                return;
            }

            File.WriteAllBytes(outputPath, encryptedContent);
            Console.WriteLine($"Plik zaszyfrowany i zapisany do {outputPath}");

            if (!keepOriginal)
            {
                File.Delete(filePath);
                Console.WriteLine("Oryginalny plik został usunięty.");
            }
        }

        public static void HandleDecrypt(string[] args)
        {
            if (Array.Exists(args, element => element == "--help"))
            {
                HelpPrinter.PrintHelp();
                return;
            }

            string filePath = null;
            string outputPath = null;
            bool keepEncrypted = false;

            // Parsowanie argumentów
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-out":
                        outputPath = args[++i];
                        break;
                    case "-b":
                        keepEncrypted = true;
                        break;
                    default:
                        filePath = args[i];
                        break;
                }
            }

            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("Nie podano pliku wejściowego. Użyj --help dla pomocy.");
                return;
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                // Domyślna ścieżka wyjściowa, jeśli nie podano `-out`
                outputPath = Path.ChangeExtension(filePath, ".dec");
            }

            Console.Write("Podaj hasło: ");
            SecureString password = PasswordReader.ReadPassword();

            byte[] encryptedContent = File.ReadAllBytes(filePath);
            byte[] decryptedContent = null;

            // Próba deszyfrowania AES, a potem DES
            try
            {
                decryptedContent = DecryptAES(encryptedContent, SecureStringToString(password));
            }
            catch
            {
                try
                {
                    decryptedContent = DecryptDES(encryptedContent, SecureStringToString(password));
                }
                catch
                {
                    Console.WriteLine("Nie można odszyfrować pliku. Złe hasło lub algorytm.");
                    return;
                }
            }

            File.WriteAllBytes(outputPath, decryptedContent);
            Console.WriteLine($"Plik odszyfrowany i zapisany do {outputPath}");

            if (!keepEncrypted)
            {
                File.Delete(filePath);
                Console.WriteLine("Zaszyfrowany plik został usunięty.");
            }
        }

        // Metody EncryptAES, DecryptAES, EncryptDES, DecryptDES pozostają bez zmian
        private static string SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = System.Runtime.InteropServices.Marshal.SecureStringToGlobalAllocUnicode(value);
            try
            {
                return System.Runtime.InteropServices.Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        private static byte[] EncryptAES(byte[] data, string password)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(password.PadRight(32));
                aes.IV = new byte[16]; // Inicjalizacja wektora IV

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                    }
                    return ms.ToArray();
                }
            }
        }

        private static byte[] DecryptAES(byte[] data, string password)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(password.PadRight(32));
                aes.IV = new byte[16]; // Inicjalizacja wektora IV

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                    }
                    return ms.ToArray();
                }
            }
        }

        private static byte[] EncryptDES(byte[] data, string password)
        {
            using (DES des = DES.Create())
            {
                des.Key = Encoding.UTF8.GetBytes(password.PadRight(8));
                des.IV = new byte[8]; // Inicjalizacja wektora IV

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                    }
                    return ms.ToArray();
                }
            }
        }

        private static byte[] DecryptDES(byte[] data, string password)
        {
            using (DES des = DES.Create())
            {
                des.Key = Encoding.UTF8.GetBytes(password.PadRight(8));
                des.IV = new byte[8]; // Inicjalizacja wektora IV

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                    }
                    return ms.ToArray();
                }
            }
        }

    }
}



        

        

       


