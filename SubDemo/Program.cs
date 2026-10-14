using System;
using CalcClassBr;

namespace SubDemo
{
    /// <summary>
    /// Демонстраційна консольна програма для методу CalcClass.Sub (варіант 17).
    /// Дозволяє показати роботу тестованого методу «наживо» під час захисту.
    ///
    /// Запуск у режимі діалогу:        dotnet run --project SubDemo
    /// Запуск з аргументами:           dotnet run --project SubDemo -- 10 3
    /// </summary>
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (args.Length == 2)
            {
                Evaluate(args[0], args[1]);
                return;
            }

            Console.WriteLine("Демонстрація методу CalcClass.Sub (віднімання).");
            Console.WriteLine("Межі int: від -2147483648 до 2147483647.");
            Console.WriteLine("Для виходу введіть порожній рядок.");
            Console.WriteLine();

            while (true)
            {
                Console.Write("Введіть зменшуване (a): ");
                string first = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(first)) break;

                Console.Write("Введіть від'ємник (b): ");
                string second = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(second)) break;

                Evaluate(first, second);
                Console.WriteLine();
            }
        }

        private static void Evaluate(string rawA, string rawB)
        {
            long a, b;

            if (!long.TryParse(rawA, out a) || !long.TryParse(rawB, out b))
            {
                Console.WriteLine("  Помилка: введені дані не є цілими числами.");
                return;
            }

            try
            {
                int result = CalcClass.Sub(a, b);
                Console.WriteLine("  Результат: {0} - {1} = {2}", a, b, result);
                Console.WriteLine("  lastError: \"{0}\" (порожній — помилки немає)", CalcClass.lastError);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("  Виняток: ArgumentOutOfRangeException");
                Console.WriteLine("  lastError: {0}", CalcClass.lastError);
            }
        }
    }
}
