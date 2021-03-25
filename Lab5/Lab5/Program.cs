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
            //TOPSIS topsis = ReadTOPSIS();
            //WriteTOPSISResults(topsis);

            VIKOR vikor = ReadVIKOR();
            WriteVIKORResults(vikor);
        }

        public static void WriteTOPSISResults(TOPSIS topsis)
        {
            Console.WriteLine("Нормалiзованi оцiнки альтернатив:");
            PrintEvaluations(topsis.AlternativesCount, topsis.CriteriasCount, () => topsis.NormalizedEvaluations);
            Console.WriteLine();
            Console.WriteLine("Зваженi нормалiзованi оцiнки альтернатив:");
            PrintEvaluations(topsis.AlternativesCount, topsis.CriteriasCount, () => topsis.WeightedNormalizedEvaluations);
            Console.WriteLine();
            Console.WriteLine($"PIS (утопiчна точка): {string.Join(' ', topsis.PIS.Select(elem => string.Format("{0:0.000}", elem)))}");
            Console.WriteLine();
            Console.WriteLine($"NIS (антиутопiчна точка): {string.Join(' ', topsis.NIS.Select(elem => string.Format("{0:0.000}", elem)))}");
            Console.WriteLine();
            Console.WriteLine($"D+ (вiдстань до PIS): {string.Join(' ', topsis.DToPIS.Select(elem => string.Format("{0:0.000}", elem)))}");
            Console.WriteLine();
            Console.WriteLine($"D- (вiдстань до NIS): {string.Join(' ',topsis.DToNIS.Select(elem => string.Format("{0:0.000}", elem)))}");
            Console.WriteLine();
            Console.WriteLine($"С (наближенiсть до PIS): ");
            Console.WriteLine($"{string.Join(Environment.NewLine, topsis.C.Select(elem => string.Format("(альтернатива: {0}, наближенiсть: {1:0.000})", elem.alternative, elem.closennes)))}");
            Console.WriteLine();
        }

        public static void WriteVIKORResults(VIKOR vikor)
        {
            Console.WriteLine("Нормалiзованi оцiнки альтернатив:");
            PrintEvaluations(vikor.AlternativesCount, vikor.CriteriasCount, () => vikor.NormalizedEvaluations);
            Console.WriteLine();
            Console.WriteLine("Зваженi нормалiзованi оцiнки альтернатив:");
            PrintEvaluations(vikor.AlternativesCount, vikor.CriteriasCount, () => vikor.WeightedNormalizedEvaluations);
            Console.WriteLine();
            Console.WriteLine($"S: ");
            Console.WriteLine($"{string.Join(Environment.NewLine, vikor.S/*.OrderBy(pair => pair.sValue)*/.Select(elem => string.Format("(альтернатива: {0}, наближенiсть: {1:0.000})", elem.alternative, elem.sValue)))}");
            Console.WriteLine();
            Console.WriteLine($"R: ");
            Console.WriteLine($"{string.Join(Environment.NewLine, vikor.R/*.OrderBy(pair => pair.rValue)*/.Select(elem => string.Format("(альтернатива: {0}, наближенiсть: {1:0.000})", elem.alternative, elem.rValue)))}");
            Console.WriteLine();
            Console.WriteLine($"Q: ");
            Console.WriteLine($"{string.Join(Environment.NewLine, vikor.Q/*.OrderBy(pair => pair.qValue)*/.Select(elem => string.Format("(альтернатива: {0}, наближенiсть: {1:0.000})", elem.alternative, elem.qValue)))}");
            Console.WriteLine();
            Console.WriteLine($"C1: {string.Join(' ', vikor.C1)}");
            Console.WriteLine();
            Console.WriteLine($"C2: {string.Join(' ', vikor.C2)}");
            Console.WriteLine();
            Console.WriteLine($"Результат: {string.Join(' ', vikor.FinalResult)}");
            Console.WriteLine();
        }

        public static void PrintEvaluations(int rowsCount, int columnsCount, Func<double[][]> printingSelector)
        {
            double[][] toPrint = printingSelector();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"   {string.Join("     ", Enumerable.Range(0, columnsCount))}");
            for (int i = 0; i < rowsCount; i++)
            {
                Console.Write($"{i}{string.Concat(Enumerable.Repeat(' ', 3 - i.ToString().Length))}");
                Console.ForegroundColor = ConsoleColor.White;
                for (int j = 0; j < columnsCount; j++)
                {
                    Console.Write($"{string.Format("{0:0.000}", toPrint[i][j])}{string.Concat(Enumerable.Repeat(' ', (j + 1).ToString().Length))}");
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

            int alternativesCount = 4;
            int[][] relation = new int[alternativesCount][];
            for (int i = 1; i < alternativesCount + 1; i++)
            {
                relation[i - 1] = allFileLines[i].Split(' ')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => int.Parse(s))
                    .ToArray();
            }

            List<double> weights = new List<double> { 0.3, 0.4, 0.3 };
            List<int> weightsToMax = new List<int> { 0, 1 };
            List<int> weightsToMin = new List<int> { 2, 3 };

            return new TOPSIS(relation, weights, weightsToMax, weightsToMin);
        }

        public static VIKOR ReadVIKOR()
        {
            string directoryPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string fileName = $"{directoryPath}\\var11_task.txt";
            string[] allFileLines = File.ReadAllLines(fileName);

            int alternativesCount = 4;
            int[][] relation = new int[alternativesCount][];
            for (int i = 1; i < alternativesCount + 1; i++)
            {
                relation[i - 1] = allFileLines[i].Split(' ')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => int.Parse(s))
                    .ToArray();
            }

            List<double> weights = new List<double> { 0.3, 0.4, 0.3 };

            return new VIKOR(relation, vCoef: 0.5, weights);
        }
    }
}
