using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lab5
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        public void WriteTOPSISResults(TOPSIS topsis)
        {
            Console.WriteLine("Нормалізовані оцінки альтернатив:");
            PrintTopsis(topsis, () => topsis.NormalizedEvaluations);
            Console.WriteLine();
            Console.WriteLine("Зважені нормалізовані оцінки альтернатив:");
            PrintTopsis(topsis, () => topsis.WeightedNormalizedEvaluations);
            Console.WriteLine();
            Console.WriteLine($"PIS (утопічна точка): {topsis.PIS.Select(elem => string.Format("{0:0.000}", elem))}");
            Console.WriteLine();
            Console.WriteLine($"NIS (антиутопічна точка): {topsis.NIS.Select(elem => string.Format("{0:0.000}", elem))}");
            Console.WriteLine();
            Console.WriteLine($"D+ (відстань до PIS): {topsis.DToPIS.Select(elem => string.Format("{0:0.000}", elem))}");
            Console.WriteLine();
            Console.WriteLine($"D- (відстань до NIS): {topsis.DToNIS.Select(elem => string.Format("{0:0.000}", elem))}");
            Console.WriteLine();
            Console.WriteLine($"С (наближеність до PIS): {topsis.C.Select(elem => string.Format("{0:0.000}", elem))}");
            Console.WriteLine();
        }

        public static void PrintTopsis(TOPSIS topsis, Func<double[][]> printingSelector)
        {
            double[][] toPrint = printingSelector();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"   {string.Join(' ', Enumerable.Range(0, topsis.CriteriasCount))}");
            for (int i = 0; i < topsis.AlternativesCount; i++)
            {
                Console.Write($"{i}{string.Concat(Enumerable.Repeat(' ', 3 - i.ToString().Length))}");
                Console.ForegroundColor = ConsoleColor.White;
                for (int j = 0; j < topsis.CriteriasCount; j++)
                {
                    Console.Write($"{toPrint[i][j]}{string.Concat(Enumerable.Repeat(' ', (j + 1).ToString().Length))}");
                }
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        public static TOPSIS ReadTOPSIS()
        {
            string directoryPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string fileName = $"{directoryPath}\\var11_task.txt";
            string[] allFileLines = File.ReadAllLines(fileName);

            int alternativesCount = 15;
            int[][] relation = new int[alternativesCount][];
            for (int i = 1; i < alternativesCount + 1; i++)
            {
                relation[i - 1] = allFileLines[i].Split(' ')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => int.Parse(s))
                    .ToArray();
            }

            List<double> weights = new List<double> { 1, 10, 5, 2, 2, 6, 2, 5, 8, 2, 5, 8 };
            List<int> weightsToMax = new List<int> { 0, 1, 2, 3, 4, 5, 6 };
            List<int> weightsToMin = new List<int> { 7, 8, 9, 10, 11 };

            return new TOPSIS(relation, weights);
        }
    }
}
