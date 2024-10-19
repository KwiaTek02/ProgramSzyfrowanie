namespace FileEncryptor
{
    public static class HelpPrinter
    {
        public static void PrintHelp()
        {
            Console.WriteLine("Użycie:");
            Console.WriteLine("  szyfruj [plik] [-a algorytm] [-out ścieżka wyjściowa] [-b]");
            Console.WriteLine("  deszyfruj [plik] [-out ścieżka wyjściowa] [-b]");
            Console.WriteLine();
            Console.WriteLine("Dostępne algorytmy:");
            Console.WriteLine("  des  - Szyfrowanie DES");
            Console.WriteLine("  aes  - Szyfrowanie AES (domyślny)");
        }
    }
}