using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Relation> relations = ReadAllRelations();
            
            var bfsCycleFinder = new BfsCycleFinder();
            var nmOptimization = new NMOptimization();
            var kOptimization = new KOptimization();

            Relation relation;
            bool hasCycle;
            HashSet<int> bestAlternatives;
            for (int relationNum = 1; relationNum <= relations.Count; relationNum++)
            {
                relation = relations[relationNum - 1];
                Console.WriteLine($"R{relationNum}{string.Concat(Enumerable.Repeat('-', 50))}");
                PrintRelation(relation);

                hasCycle = bfsCycleFinder.HasCycle(relation);
                Console.WriteLine($"Є ациклiчним: {!hasCycle}");

                if (!hasCycle)
                {
                    bestAlternatives = nmOptimization.GetBestAlternatives(relation);
                    Console.WriteLine($"Розв’язок Неймана-Моргенштерна: {string.Join(" ", bestAlternatives)}");
                    Console.WriteLine($"Чи є розв’язок внутрiшньо стiйким: {nmOptimization.IsBestAlternativesInternallyStable(bestAlternatives, relation)}");
                    Console.WriteLine($"Чи є розв’язок зовнiшньо стiйким: {nmOptimization.IsBestAlternativesExternallyStable(bestAlternatives, relation)}");
                }
                else
                {
                    Console.WriteLine("Опт. альтернативи за принципом К - оптимiзацiї");
                    bestAlternatives = kOptimization.GetK1BestAlternatives(relation);
                    Console.WriteLine($"K = 1: {string.Join(" ", bestAlternatives)}");
                    bestAlternatives = kOptimization.GetK1OptAlternatives(bestAlternatives, relation);
                    Console.WriteLine($"K = 1 (opt): {string.Join(" ", bestAlternatives)}");
                    bestAlternatives = kOptimization.GetK2BestAlternatives(relation);
                    Console.WriteLine($"K = 2: {string.Join(" ", bestAlternatives)}");
                    bestAlternatives = kOptimization.GetK3BestAlternatives(relation);
                    Console.WriteLine($"K = 3: {string.Join(" ", bestAlternatives)}");
                    bestAlternatives = kOptimization.GetK4BestAlternatives(relation);
                    Console.WriteLine($"K = 4: {string.Join(" ", bestAlternatives)}");
                }
                Console.WriteLine();
                Console.WriteLine();
            }

            Console.WriteLine(relations[3].CharateristicToString());
        }

        public static List<Relation> ReadAllRelations()
        {
            string directoryPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string fileName = $"{directoryPath}\\relations_var11.txt";
            string[] allFileLines = File.ReadAllLines(fileName);

            List<Relation> relations = new List<Relation>(10);
            int[][] relation = new int[15][];
            for (int i = 1; i < allFileLines.Length; i++)
            {
                if (i % 16 == 0)
                {
                    relations.Add(new Relation(relation));
                    relation = new int[15][];
                    continue;
                }

                relation[(i - 1 - relations.Count) % 15] = allFileLines[i].Split(' ')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => int.Parse(s))
                    .ToArray();
            }
            relations.Add(new Relation(relation));

            return relations;
        }

        public static void PrintRelation(Relation relation)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"   {string.Join(' ', Enumerable.Range(0, 15))}");
            for(int i = 0; i < relation.Dimension; i++)
            {
                Console.Write($"{i}{string.Concat(Enumerable.Repeat(' ', 3 - i.ToString().Length))}");
                Console.ForegroundColor = ConsoleColor.White;
                for(int j = 0; j < relation.Dimension; j++)
                {
                    Console.Write($"{relation.Connections[i][j]}{string.Concat(Enumerable.Repeat(' ', (j + 1).ToString().Length))}");
                    
                }
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }
    }
}
