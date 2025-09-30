using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Globalization;

namespace Finance
{
    // kategorijas izdevumiem
    enum Cat { Food, Transport, Fun, School, Other }

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
// parbaude lai nav visu laiku copy paste
    class ValidationException : Exception
    {
        public ValidationException(string msg) : base(msg) { }
    }

    static class Tools
    {
        public static decimal SafeParseDecimal(string msg)
        {
            while (true)
            {
                Console.Write(msg);
                if (decimal.TryParse(Console.ReadLine(), out var d) && d > 0)
                    return d;
                Console.WriteLine("Nepareiza summa — ievadi pozitīvu skaitli.");
            }
        }

        public static string ReadNonEmptyString(string msg)
        {
            while (true)
            {
                Console.Write(msg);
                string s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s)) return s;
                Console.WriteLine("Teksts nevar būt tukšs!");
            }
        }

        public static DateTime ReadDateOptional(string msg)
        {
            // Lietotājs var atstāt tukšu, tad tiek DateTime now
            Console.Write(msg + " (tukšs = šodien, formāts yyyy-MM-dd): ");
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return DateTime.Now;
            if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt;
            Console.WriteLine("Nederīgs datums — izmanto formātu yyyy-MM-dd. Tiks prasīts vēlreiz.");
            return ReadDateOptional(msg);
        }

        public static int ReadCategory(string msg)
        {
            while (true)
            {
                Console.Write(msg);
                var s = Console.ReadLine();
                if (int.TryParse(s, out var k) && k >= 1 && k <= Enum.GetValues(typeof(Cat)).Length)
                    return k;
                Console.WriteLine($"Nederīga izvēle. Ievadi skaitli no 1 līdz {Enum.GetValues(typeof(Cat)).Length}.");
            }
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
                Console.WriteLine("=== FINANŠU LIETOTNE ===");
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
                    else { Console.WriteLine("Nezināma opcija."); Console.ReadKey(); }
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
            DateTime date = Tools.ReadDateOptional("Datums");
            inc.Add(new Income() { Date = date, Source = s, Amount = d });
            Console.WriteLine("Ienākums pievienots.");
            Console.ReadKey();
        }

        static void DelInc()
        {
            Console.Clear();
            for (int i = 0; i < inc.Count; i++)
                Console.WriteLine($"{i + 1}) {inc[i].Date:yyyy-MM-dd} {inc[i].Source} {inc[i].Amount}€");
            Console.Write("Nr dzēšanai: ");
            if (int.TryParse(Console.ReadLine(), out int n) && n > 0 && n <= inc.Count)
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
            int k = Tools.ReadCategory("Izvēlies kategoriju (1-5): ");
            decimal amt = Tools.SafeParseDecimal("Summa: ");
            string n = Tools.ReadNonEmptyString("Piezīme: ");
            DateTime date = Tools.ReadDateOptional("Datums");
            exp.Add(new Expense() { Date = date, Category = (Cat)(k - 1), Amount = amt, Note = n });
            Console.WriteLine("Izdevums pievienots.");
            Console.ReadKey();
        }

        static void DelExp()
        {
            Console.Clear();
            for (int i = 0; i < exp.Count; i++)
                Console.WriteLine($"{i + 1}) {exp[i].Date:yyyy-MM-dd} {exp[i].Category} {exp[i].Amount}€ {exp[i].Note}");
            Console.Write("Nr dzēšanai: ");
            if (int.TryParse(Console.ReadLine(), out int n) && n > 0 && n <= exp.Count)
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
            Console.WriteLine("1) Pievienot abonementu");
            Console.WriteLine("2) Rādīt abonementus");
            Console.WriteLine("3) Mainīt statusu (aktīvs/nea) ");
            Console.WriteLine("4) Dzēst abonementu");
            string cc = Console.ReadLine();
            if (cc == "1")
            {
                string n = Tools.ReadNonEmptyString("Nosaukums: ");
                decimal p = Tools.SafeParseDecimal("Cena: ");
                DateTime start = Tools.ReadDateOptional("Sākuma datums");
                subs.Add(new Subscription() { Name = n, MonthlyPrice = p, StartDate = start, IsActive = true });
                Console.WriteLine("Abonements pievienots.");
            }
            else if (cc == "2")
            {
                foreach (var s in subs) Console.WriteLine($"{s.Name} {s.MonthlyPrice}€ {(s.IsActive ? "aktīvs" : "neaktīvs")} {s.StartDate:yyyy-MM-dd}");
            }
            else if (cc == "3")
            {
                Console.Write("Nosaukums kuru mainīt: ");
                string nn = Console.ReadLine();
                var ss = subs.FirstOrDefault(x => x.Name == nn);
                if (ss != null)
                {
                    ss.IsActive = !ss.IsActive;
                    Console.WriteLine($"Statuss nomainīts uz {(ss.IsActive ? "aktīvs" : "neaktīvs")}.");
                }
                else Console.WriteLine("Nav tāda abonementa.");
            }
            else if (cc == "4")
            {
                Console.Write("Nosaukums dzēšanai: ");
                string n = Console.ReadLine();
                int removed = subs.RemoveAll(x => x.Name == n);
                Console.WriteLine(removed > 0 ? "Dzēsts." : "Nav tāda.");
            }
            Console.ReadKey();
        }

        static void ShowAll()
        {
            Console.Clear();
            Console.WriteLine("== IENĀKUMI ==");
            foreach (var i in inc.OrderByDescending(x => x.Date))
                Console.WriteLine($"{i.Date:yyyy-MM-dd} {i.Source,-20} {i.Amount,8}€");
            Console.WriteLine("\n== IZDEVUMI ==");
            foreach (var e in exp.OrderByDescending(x => x.Date))
                Console.WriteLine($"{e.Date:yyyy-MM-dd} {e.Category,-10} {e.Amount,8}€ {e.Note}");
            Console.WriteLine("\n== ABONEMENTI ==");
            foreach (var s in subs.OrderByDescending(x => x.StartDate))
                Console.WriteLine($"{s.Name,-20} {s.MonthlyPrice,8}€ {(s.IsActive ? "aktīvs" : "neaktīvs")} {s.StartDate:yyyy-MM-dd}");
            Console.ReadKey();
        }

        static void Filters()
        {
            Console.Clear();
            Console.WriteLine("1) Pēc kategorijas (izdevumi)");
            Console.WriteLine("2) Pēc datuma diapazona (ienākumi vai izdevumi)");
            Console.WriteLine("3) Ienākumu filtrs pēc avota");
            string c = Console.ReadLine();

            if (c == "1")
            {
                Console.WriteLine("Kategorija: 1-Food 2-Transport 3-Fun 4-School 5-Other");
                int k = Tools.ReadCategory("Izvēlies kategoriju (1-5): ");
                var res = exp.Where(x => x.Category == (Cat)(k - 1));
                decimal total = res.Sum(x => x.Amount);
                foreach (var f in res) Console.WriteLine($"{f.Date:yyyy-MM-dd} {f.Category} {f.Amount}€ {f.Note}");
                Console.WriteLine($"Kopā: {total}€");
            }
            else if (c == "2")
            {
                Console.Write("No (yyyy-mm-dd): ");
                if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var from))
                {
                    Console.WriteLine("Nederīgs datums."); Console.ReadKey(); return;
                }
                Console.Write("Līdz (yyyy-mm-dd): ");
                if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var to))
                {
                    Console.WriteLine("Nederīgs datums."); Console.ReadKey(); return;
                }

                var expRes = exp.Where(x => x.Date >= from && x.Date <= to);
                var incRes = inc.Where(x => x.Date >= from && x.Date <= to);
                Console.WriteLine("-- Izdevumi --");
                foreach (var f in expRes) Console.WriteLine($"{f.Date:yyyy-MM-dd} {f.Category} {f.Amount}€ {f.Note}");
                Console.WriteLine($"Kopā izdevumi: {expRes.Sum(x => x.Amount)}€");
                Console.WriteLine("-- Ienākumi --");
                foreach (var f in incRes) Console.WriteLine($"{f.Date:yyyy-MM-dd} {f.Source} {f.Amount}€");
                Console.WriteLine($"Kopā ienākumi: {incRes.Sum(x => x.Amount)}€");
            }
            else if (c == "3")
            {
                string src = Tools.ReadNonEmptyString("Avots (daļa vai pilns nosaukums): ");
                var res = inc.Where(x => x.Source.IndexOf(src, StringComparison.CurrentCultureIgnoreCase) >= 0);
                foreach (var i in res) Console.WriteLine($"{i.Date:yyyy-MM-dd} {i.Source} {i.Amount}€");
                Console.WriteLine($"Kopā: {res.Sum(x => x.Amount)}€");
            }

            Console.ReadKey();
        }

        static void MonthReport()
        {
            Console.Clear();
            Console.WriteLine("Ievadi mēnesi (YYYY-MM) vai atstāj tukšu, lai izmantotu pašreizējo mēnesi:");
            Console.Write("Mēnesis: ");
            string input = Console.ReadLine();
            int year, month;
            if (string.IsNullOrWhiteSpace(input))
            {
                var now = DateTime.Now;
                year = now.Year; month = now.Month;
            }
            else if (DateTime.TryParseExact(input + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                year = parsed.Year; month = parsed.Month;
            }
            else
            {
                Console.WriteLine("Nederīgs formāts. Lieto YYYY-MM."); Console.ReadKey(); return;
            }

            var monthInc = inc.Where(i => i.Date.Year == year && i.Date.Month == month).Sum(x => x.Amount);
            var monthExpList = exp.Where(i => i.Date.Year == year && i.Date.Month == month).ToList();
            var activeSubs = subs.Where(s => s.IsActive).Sum(s => s.MonthlyPrice);

            Console.WriteLine($"Ienākumi: {monthInc}€");
            Console.WriteLine($"Izdevumi: {monthExpList.Sum(x => x.Amount)}€");
            Console.WriteLine($"Abonementi (aktīvie): {activeSubs}€");
            Console.WriteLine($"Bilance: {monthInc - monthExpList.Sum(x => x.Amount) - activeSubs}€");

            if (monthExpList.Any())
            {
                var big = monthExpList.OrderByDescending(x => x.Amount).First();
                Console.WriteLine($"Lielākais izdevums: {big.Category} {big.Amount}€ ({big.Note})");
                decimal avg = Tools.SafeDivide(monthExpList.Sum(x => x.Amount), DateTime.DaysInMonth(year, month));
                Console.WriteLine($"Vidējais dienas tēriņš: {Math.Round(avg, 2)}€");
                Console.WriteLine("-- Kategoriju % --");
                decimal monthTotal = monthExpList.Sum(x => x.Amount);
                foreach (Cat c in Enum.GetValues(typeof(Cat)))
                {
                    var sum = monthExpList.Where(x => x.Category == c).Sum(x => x.Amount);
                    Console.WriteLine($"{c}: {Tools.Percent(sum, monthTotal)}%");
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
