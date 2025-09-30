using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json; // lai JSON strādātu
// using System.Text.Json.Serialization; // varbūt vajag vēlāk

namespace Finance
{
    enum Cat { Food, Transport, Fun, School, Other } // kategorijas prieks izdevumiem.
// klases, ienakumi ect.
    class Income
    {
        public DateTime Date { get; set; }
        public string Source { get; set; }
        public decimal Amount { get; set; }
    }

    class Expense
    {
        public DateTime Date { get; set; }
        public Cat Category { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; }
    }

    class Subscription
    {
        public string Name { get; set; }
        public decimal MonthlyPrice { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }
    }
    //parbauda vai lietotajs ievadija pareizu info, nav negativs text vai tukss.
    class ValidationException : Exception
    {
        public ValidationException(string msg) : base(msg) { }
    }
    //sature metodes kas var noderet vairakas reizes, lai nav copy paste.
    static class Tools
    {
        public static decimal SafeParseDecimal(string msg)
        {
            Console.Write(msg);
            decimal d;
            if (!decimal.TryParse(Console.ReadLine(), out d) || d <= 0)
                throw new ValidationException("Nepareiza summa!");
            return d;
        }

        public static string ReadNonEmptyString(string msg)
        {
            Console.Write(msg);
            string s = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(s)) throw new ValidationException("Teksts nevar būt tukšs!");
            return s;
        }

        public static decimal SafeDivide(decimal a, decimal b)
        {
            if (b == 0) return 0;
            return a / b;
        }

        public static decimal Percent(decimal part, decimal total)
        {
            if (total == 0) return 0;
            return Math.Round((part / total) * 100, 1);
        }
    }
    // galvena klase jeb tas kas glaba visu utt. idk kaut kas.
    class Program
    {
        static List<Income> inc = new List<Income>();
        static List<Expense> exp = new List<Expense>();
        static List<Subscription> subs = new List<Subscription>();

        static void Main(string[] args)
        {
            bool run = true;
            while (run)
            {
                Console.Clear();
                Console.WriteLine("=== FINANŠU LIETOTNE===");
                Console.WriteLine("1) Ienākumi");
                Console.WriteLine("2) Izdevumi");
                Console.WriteLine("3) Abonementi");
                Console.WriteLine("4) Saraksti");
                Console.WriteLine("5) Filtri");
                Console.WriteLine("6) Mēneša pārskats");
                Console.WriteLine("7) Import/Export JSON");
                Console.WriteLine("0) Iziet");

                string c = Console.ReadLine();

                try
                {
                    if (c == "1") IncMenu();
                    else if (c == "2") ExpMenu();
                    else if (c == "3") SubMenu();
                    else if (c == "4") ShowAll();
                    else if (c == "5") Filters();
                    else if (c == "6") MonthReport();
                    else if (c == "7") JsonStuff();
                    else if (c == "0") run = false;
                    else { Console.WriteLine("??"); Console.ReadKey(); }
                }
                catch (ValidationException ve)
                {
                    Console.WriteLine("Kļūda: " + ve.Message);
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Kaut kas salūza: " + ex.Message);
                    Console.ReadKey();
                }
            }
        }

        static void IncMenu()
        {
            Console.Clear();
            Console.WriteLine("1) Pievienot ienākumu");
            Console.WriteLine("2) Dzēst ienākumu");
            string c = Console.ReadLine();
            if (c == "1") AddInc();
            else if (c == "2") DelInc();
        }

        static void AddInc()
        {
            Console.Clear();
            string s = Tools.ReadNonEmptyString("Avots: ");
            decimal d = Tools.SafeParseDecimal("Summa: ");
            inc.Add(new Income() { Date = DateTime.Now, Source = s, Amount = d });
            Console.WriteLine("Ok pievienots.");
            Console.ReadKey();
        }

        static void DelInc()
        {
            Console.Clear();
            for (int i = 0; i < inc.Count; i++)
                Console.WriteLine($"{i + 1}) {inc[i].Source} {inc[i].Amount}€");
            Console.Write("Nr dzēšanai: ");
            int n;
            if (int.TryParse(Console.ReadLine(), out n) && n > 0 && n <= inc.Count)
            {
                inc.RemoveAt(n - 1);
                Console.WriteLine("Dzēsts.");
            }
            else Console.WriteLine("Nav tāda.");
            Console.ReadKey();
        }

        static void ExpMenu()
        {
            Console.Clear();
            Console.WriteLine("1) Pievienot izdevumu");
            Console.WriteLine("2) Dzēst izdevumu");
            string c = Console.ReadLine();
            if (c == "1") AddExp();
            else if (c == "2") DelExp();
        }

        static void AddExp()
        {
            Console.Clear();
            Console.WriteLine("Kategorija: 1-Food 2-Transport 3-Fun 4-School 5-Other");
            int k = int.Parse(Console.ReadLine()); // speciāli nav TryParse
            decimal amt = Tools.SafeParseDecimal("Summa: ");
            string n = Tools.ReadNonEmptyString("Piezīme: ");
            exp.Add(new Expense() { Date = DateTime.Now, Category = (Cat)(k - 1), Amount = amt, Note = n });
            Console.WriteLine("Izdevums gatavs.");
            Console.ReadKey();
        }

        static void DelExp()
        {
            Console.Clear();
            for (int i = 0; i < exp.Count; i++)
                Console.WriteLine($"{i + 1}) {exp[i].Category} {exp[i].Amount}€ {exp[i].Note}");
            Console.Write("Nr dzēšanai: ");
            int n;
            if (int.TryParse(Console.ReadLine(), out n) && n > 0 && n <= exp.Count)
            {
                exp.RemoveAt(n - 1);
                Console.WriteLine("Dzēsts.");
            }
            else Console.WriteLine("Nav tāda.");
            Console.ReadKey();
        }

        static void SubMenu()
        {
            Console.Clear();
            Console.WriteLine("1) Add sub");
            Console.WriteLine("2) Show subs");
            Console.WriteLine("3) Toggle status");
            Console.WriteLine("4) Delete sub");
            string cc = Console.ReadLine();
            if (cc == "1")
            {
                Console.Write("Nosaukums: "); string n = Tools.ReadNonEmptyString("");
                decimal p = Tools.SafeParseDecimal("Cena: ");
                subs.Add(new Subscription() { Name = n, MonthlyPrice = p, StartDate = DateTime.Now, IsActive = true });
                Console.WriteLine("Sub pievienots");
            }
            else if (cc == "2")
            {
                foreach (var s in subs) Console.WriteLine($"{s.Name} {s.MonthlyPrice}€ {(s.IsActive ? "on" : "off")}");
            }
            else if (cc == "3")
            {
                Console.Write("Nosaukums kuru mainīt: ");
                string nn = Console.ReadLine();
                var ss = subs.FirstOrDefault(x => x.Name == nn);
                if (ss != null) ss.IsActive = !ss.IsActive;
            }
            else if (cc == "4")
            {
                Console.Write("Nosaukums dzēšanai: ");
                string n = Console.ReadLine();
                subs.RemoveAll(x => x.Name == n);
                Console.WriteLine("Dzēsts ja bija.");
            }
            Console.ReadKey();
        }

        static void ShowAll()
        {
            Console.Clear();
            Console.WriteLine("== IENĀKUMI ==");
            foreach (var i in inc.OrderByDescending(x => x.Date))
                Console.WriteLine($"{i.Date} {i.Source} {i.Amount}€");
            Console.WriteLine("== IZDEVUMI ==");
            foreach (var e in exp.OrderByDescending(x => x.Date))
                Console.WriteLine($"{e.Date} {e.Category} {e.Amount}€ {e.Note}");
            Console.WriteLine("== ABONEMENTI ==");
            foreach (var s in subs.OrderByDescending(x => x.StartDate))
                Console.WriteLine($"{s.Name} {s.MonthlyPrice}€ {(s.IsActive ? "ok" : "x")}");
            Console.ReadKey();
        }
        //sitas pigors lauj tev atlasit lietas - piem tikai ediens
        static void Filters()
        {
            Console.Clear();
            Console.WriteLine("1) Pēc kategorijas");
            Console.WriteLine("2) Pēc datuma diapazona");
            string c = Console.ReadLine();

            IEnumerable<Expense> res = exp;
            if (c == "1")
            {
                Console.WriteLine("Kategorija: 1-Food 2-Transport 3-Fun 4-School 5-Other");
                int k = int.Parse(Console.ReadLine());
                res = exp.Where(x => x.Category == (Cat)(k - 1));
            }
            else if (c == "2")
            {
                Console.Write("No (yyyy-mm-dd): ");
                DateTime from = DateTime.Parse(Console.ReadLine());
                Console.Write("Līdz (yyyy-mm-dd): ");
                DateTime to = DateTime.Parse(Console.ReadLine());
                res = exp.Where(x => x.Date >= from && x.Date <= to);
            }

            decimal total = res.Sum(x => x.Amount);
            foreach (var f in res)
                Console.WriteLine($"{f.Date} {f.Category} {f.Amount}€ {f.Note}");
            Console.WriteLine($"Kopā: {total}€");
            Console.ReadKey();
        }
// menesa pigors, cik daudz naudas ienaca un cik iztērēja.
        static void MonthReport()
        {
            Console.Clear();
            DateTime now = DateTime.Now;
            var thisMonthInc = inc.Where(i => i.Date.Month == now.Month && i.Date.Year == now.Year).Sum(x => x.Amount);
            var thisMonthExp = exp.Where(i => i.Date.Month == now.Month && i.Date.Year == now.Year).ToList();
            var activeSubs = subs.Where(s => s.IsActive).Sum(s => s.MonthlyPrice);

            Console.WriteLine($"Ienākumi: {thisMonthInc}€");
            Console.WriteLine($"Izdevumi: {thisMonthExp.Sum(x => x.Amount)}€");
            Console.WriteLine($"Abonementi: {activeSubs}€");
            Console.WriteLine($"Bilance: {thisMonthInc - thisMonthExp.Sum(x => x.Amount) - activeSubs}€");

            if (thisMonthExp.Any())
            {
                var big = thisMonthExp.OrderByDescending(x => x.Amount).First();
                Console.WriteLine($"Lielākais izdevums: {big.Category} {big.Amount}€");
                decimal avg = Tools.SafeDivide(thisMonthExp.Sum(x => x.Amount), DateTime.DaysInMonth(now.Year, now.Month));
                Console.WriteLine($"Vidējais dienas tēriņš: {Math.Round(avg, 2)}€");
                Console.WriteLine("-- Kategoriju % --");
                foreach (Cat c in Enum.GetValues(typeof(Cat)))
                {
                    var sum = thisMonthExp.Where(x => x.Category == c).Sum(x => x.Amount);
                    Console.WriteLine($"{c}: {Tools.Percent(sum, thisMonthExp.Sum(x => x.Amount))}%");
                }
            }

            Console.ReadKey();
        }

        static void JsonStuff()
        {
            Console.Clear();
            Console.WriteLine("1) Export");
            Console.WriteLine("2) Import");
            string c = Console.ReadLine();
            if (c == "1")
            {
                var obj = new { Incomes = inc, Expenses = exp, Subs = subs };
                string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(json);
            }
            else if (c == "2")
            {
                Console.WriteLine("Ievadi JSON:");
                string j = Console.ReadLine();
                try
                {
                    var data = JsonSerializer.Deserialize<TempData>(j);
                    if (data != null)
                    {
                        inc = data.Incomes ?? inc;
                        exp = data.Expenses ?? exp;
                        subs = data.Subs ?? subs;
                        Console.WriteLine("Import ok");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Import fail: " + ex.Message);
                }
            }
            Console.ReadKey();
        }

        class TempData
        {
            public List<Income> Incomes { get; set; }
            public List<Expense> Expenses { get; set; }
            public List<Subscription> Subs { get; set; }
        }
    }
}


