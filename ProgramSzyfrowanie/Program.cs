using System;

namespace FileEncryptor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || (args.Length > 0 && args[0] == "--help"))
            {
                HelpPrinter.PrintHelp();
                return;
            }

            // Sprawdzenie pierwszego argumentu na podstawie komendy (szyfruj/deszyfruj)
            switch (args[0])
            {
                case "szyfruj":
                    Encryptor.HandleEncrypt(args);
                    break;
                case "deszyfruj":
                    Encryptor.HandleDecrypt(args);
                    break;
                default:
                    Console.WriteLine("Nieznana komenda. Użyj --help dla pomocy.");
                    break;
            }
        }
    }
}