using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            CriteriaRelation criteriaRelation = ReadCriteriaRelation();
            Console.WriteLine("Сигма вектори:");
            PrintCriteriaRelationVectors(criteriaRelation, () => criteriaRelation.SigmaVectors);
            Console.WriteLine("Вiдношення Парето:");
            PrintRelation(criteriaRelation.ParetoRelation, () => criteriaRelation.ParetoRelation.Connections);
            PrintRelation(criteriaRelation.ParetoRelation, () => criteriaRelation.ParetoRelation.Characteristic);
            Console.WriteLine("Мажоритарне вiдношення:");
            PrintRelation(criteriaRelation.MajorityRelation, () => criteriaRelation.MajorityRelation.Connections);
            PrintRelation(criteriaRelation.MajorityRelation, () => criteriaRelation.MajorityRelation.Characteristic);
            Console.WriteLine("Лексикографiчне вiдношення:");
            PrintRelation(criteriaRelation.LexicographicRelation, () => criteriaRelation.LexicographicRelation.Connections);
            PrintRelation(criteriaRelation.LexicographicRelation, () => criteriaRelation.LexicographicRelation.Characteristic);
            Console.WriteLine("Вiдношення Березовського:");
            PrintRelation(criteriaRelation.BerezovskyRelation, () => criteriaRelation.BerezovskyRelation.Connections);
            PrintRelation(criteriaRelation.BerezovskyRelation, () => criteriaRelation.BerezovskyRelation.Characteristic);
        }

        public static CriteriaRelation ReadCriteriaRelation()
        {
            string directoryPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string fileName = $"{directoryPath}\\relations_var11.txt";
            string[] allFileLines = File.ReadAllLines(fileName);

            int[][] relation = new int[20][];
            for (int i = 0; i < 20; i++)
            {
                relation[i] = allFileLines[i].Split(' ')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => int.Parse(s))
                    .ToArray();
            }

            HashSet<int> criteriasImportance =
                new HashSet<int> { 1, 8, 4, 10, 12, 3, 11, 5, 2, 7, 6, 9 }.Select(c => c - 1).ToHashSet();
            List<HashSet<int>> criteriasImportancesClasses =
                new List<HashSet<int>>
                {
                    new HashSet<int> { 6, 8, 12 }.Select(c => c - 1).ToHashSet(),
                    new HashSet<int> { 4, 5, 10, 11}.Select(c => c - 1).ToHashSet(),
                    new HashSet<int> { 1, 2, 3, 7, 9}.Select(c => c - 1).ToHashSet()
                };

            return new CriteriaRelation(relation, criteriasImportance, criteriasImportancesClasses);
        }

        public static void PrintRelation<T>(Relation relation, Func<T[][]> printingSelector)
        {
            T[][] toPrint = printingSelector();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"   {string.Join(' ', Enumerable.Range(0, relation.Dimension))}");
            for (int i = 0; i < relation.Dimension; i++)
            {
                Console.Write($"{i}{string.Concat(Enumerable.Repeat(' ', 3 - i.ToString().Length))}");
                Console.ForegroundColor = ConsoleColor.White;
                for (int j = 0; j < relation.Dimension; j++)
                {
                    Console.Write($"{toPrint[i][j]}{string.Concat(Enumerable.Repeat(' ', (j + 1).ToString().Length))}");
                }
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        public static void PrintCriteriaRelationVectors(CriteriaRelation criteriaRelation, Func<List<int>[][]> printingSelector)
        {
            List<int>[][] toPrint = printingSelector();

            for (int i = 0; i < criteriaRelation.AlternativesCount; i++)
            {
                for(int j = 0; j < criteriaRelation.AlternativesCount; j++)
                {
                    Console.WriteLine($"[{i}][{j}]: {string.Join(' ', toPrint[i][j])}");
                }
            }
            Console.WriteLine();
        }
    }
}
