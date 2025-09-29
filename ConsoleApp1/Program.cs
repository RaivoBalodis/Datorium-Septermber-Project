using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalFinancePlanner
{


    class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== Personīgais finanšu plānotājs ===");
                Console.WriteLine("1) Ienākumi");
                Console.WriteLine("2) Izdevumi");
                Console.WriteLine("3) Abonementi");
                Console.WriteLine("4) Saraksti");
                Console.WriteLine("5) Filtri");
                Console.WriteLine("6) Mēneša pārskats");
                Console.WriteLine("7) Import/Export JSON (nav gatavs)");
                Console.WriteLine("0) Iziet");
                string choice = Console.ReadLine();
                if (choice == "1")
                {
                    Console.WriteLine("TODO: ienākumi vēl nav pabeigti");
                }
                else if (choice == "2")
                {
                    Console.WriteLine("TODO: izdevumi vēl nav pabeigti");
                }

                else if (choice == "3")
                {
                    Console.WriteLine("TODO: abonementi vēl nav pabeigti");
                }
                else if (choice == "4")
                {
                    Console.WriteLine("TODO: saraksti vēl nav pabeigti");
                }
                else if (choice == "5")
                {
                    Console.WriteLine("TODO: filtri vēl nav pabeigti");
                }
                else if (choice == "6")
                {
                    Console.WriteLine("TODO: mēneša pārskats vēl nav pabeigts");
                }
                else if (choice == "7")
                {
                    Console.WriteLine("TODO: Import/Export JSON vēl nav pabeigts");
                }
                else if (choice == "0")
                {
                    running = false;
                }
                else
                {
                    Console.WriteLine("Nederīga izvēle. Nospiediet jebkuru taustiņu, lai turpinātu...");
                    Console.ReadKey();
                }
            }
        }

    }
}