using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            WeightedCriteriaRelation weightedCriteriaRelation = ReadWeightedCriteriaRelation();
            WriteResults(weightedCriteriaRelation);
            //Task21(weightedCriteriaRelation);
            //Task22(weightedCriteriaRelation);
            Task23(weightedCriteriaRelation);
        }

        public static WeightedCriteriaRelation ReadWeightedCriteriaRelation()
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

            return new WeightedCriteriaRelation(relation, weights, c: 0.739, d: 0.404);
        }

        public static void WriteResults(WeightedCriteriaRelation weightedCriteriaRelation)
        {
            string directoryPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string fileName = $"{directoryPath}\\Var11-КаспрукАнастасія.txt";

            File.WriteAllText(fileName, string.Empty);

            File.AppendAllLines(fileName,
                new List<string>
                {
                    "матриця індексiв узгодження C",
                    GetRelationString(weightedCriteriaRelation.CRelation),
                    "матриця індексiв неузгодження D",
                    GetRelationString(weightedCriteriaRelation.DRelation),
                    "Значення порогів для індексів узгодження та неузгодження c, d",
                    $"{weightedCriteriaRelation.C} {weightedCriteriaRelation.D}",
                    "Відношення для порогових значень c, d:",
                    weightedCriteriaRelation.BestRelation.ToString(),
                    "Ядро відношення:",
                    string.Join(' ', weightedCriteriaRelation.Core)
                });
        }

        public static string GetRelationString(double[][] relation)
        {
            return string.Join(
                Environment.NewLine, 
                relation.Select(arr => string.Join(' ', arr.Select(elem => string.Format("{0:0.000}", elem)))));
        }

        public static void Task21(WeightedCriteriaRelation weightedCriteriaRelation)
        {
            double c = 0.5;
            weightedCriteriaRelation = new WeightedCriteriaRelation(
                    weightedCriteriaRelation.Evaluations,
                    weightedCriteriaRelation.Weights.ToList(),
                    c: c,
                    d: 0);

            Console.WriteLine($"Для значення d = 0 розмiр ядра складає {weightedCriteriaRelation.Core.Count}");
            Console.WriteLine($"Поточне наповнення ядра: {string.Join(' ', weightedCriteriaRelation.Core)}");
            Console.WriteLine();

            var bfsCycleFinder = new BfsCycleFinder();
            List<double> dValues = new List<double>() { 0 };
            List<int> sizes = new List<int>() { weightedCriteriaRelation.Core.Count };
            IReadOnlyCollection<int> currentCore = weightedCriteriaRelation.Core;
            IEnumerable<int> addedCoreAlternatives;
            IEnumerable<int> removedCoreAlternatives;
            for (double d = 0.001; d <= 0.5; d = Math.Round(d + 0.001, 3, MidpointRounding.AwayFromZero))
            {
                weightedCriteriaRelation = new WeightedCriteriaRelation(
                    weightedCriteriaRelation.Evaluations,
                    weightedCriteriaRelation.Weights.ToList(),
                    c: c,
                    d: d);

                if (bfsCycleFinder.HasCycle(weightedCriteriaRelation.BestRelation))
                {
                    Console.WriteLine($"Для значення d = {string.Format("{0:0.000}", d)} у вiдношеннi для порогових значень 'c', 'd' був знайдений цикл, тому аналiз для поточного значення 'с' варто припинити");
                    break;
                }

                removedCoreAlternatives = currentCore.Except(weightedCriteriaRelation.Core);              
                addedCoreAlternatives = weightedCriteriaRelation.Core.Except(currentCore);                
                if(removedCoreAlternatives.Count() != 0 || addedCoreAlternatives.Count() != 0)
                {
                    if (weightedCriteriaRelation.Core.Count != sizes.Last())
                        Console.WriteLine($"Для значення d = {string.Format("{0:0.000}", d)} розмiр ядра змiнився з {sizes.Last()} на {weightedCriteriaRelation.Core.Count}");
                    else
                        Console.WriteLine($"Для значення d = {string.Format("{0:0.000}", d)} розмiр ядра не змiнився, але змінилося його наповнення");
                    Console.WriteLine($"Попереднє наповнення ядра: {string.Join(' ', currentCore)}");
                    Console.WriteLine($"Поточне наповнення ядра: {string.Join(' ', weightedCriteriaRelation.Core)}");
                    if (removedCoreAlternatives.Count() != 0)
                        Console.WriteLine($"З ядра були виключенi значення: {string.Join(' ', removedCoreAlternatives)}");
                    if (addedCoreAlternatives.Count() != 0)
                        Console.WriteLine($"До ядра були доданi значення: {string.Join(' ', addedCoreAlternatives)}");
                    Console.WriteLine();
                }

                dValues.Add(d);
                sizes.Add(weightedCriteriaRelation.Core.Count);
                currentCore = weightedCriteriaRelation.Core;
            }

            string directoryPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string fileName = $"{directoryPath}\\sizes-d.txt";
            File.WriteAllText(fileName, string.Empty);
            File.WriteAllLines(fileName, 
                new List<string> 
                {
                    string.Join(' ', dValues.Select(elem => string.Format("{0:0.000}", elem).Replace(",", "."))), 
                    string.Join(' ', sizes) 
                });
        }

        public static void Task22(WeightedCriteriaRelation weightedCriteriaRelation)
        {
            double d = 0.49;
            weightedCriteriaRelation = new WeightedCriteriaRelation(
                    weightedCriteriaRelation.Evaluations,
                    weightedCriteriaRelation.Weights.ToList(),
                    c: 0.5,
                    d: d);

            Console.WriteLine($"Для значення c = 0.5 розмiр ядра складає {weightedCriteriaRelation.Core.Count}");
            Console.WriteLine($"Поточне наповнення ядра: {string.Join(' ', weightedCriteriaRelation.Core)}");
            Console.WriteLine();

            var bfsCycleFinder = new BfsCycleFinder();
            List<double> cValues = new List<double>() { 0 };
            List<int> sizes = new List<int>() { weightedCriteriaRelation.Core.Count };
            IReadOnlyCollection<int> currentCore = weightedCriteriaRelation.Core;
            IEnumerable<int> addedCoreAlternatives;
            IEnumerable<int> removedCoreAlternatives;
            for (double c = 0.501; c <= 1; c = Math.Round(c + 0.001, 3, MidpointRounding.AwayFromZero))
            {
                weightedCriteriaRelation = new WeightedCriteriaRelation(
                    weightedCriteriaRelation.Evaluations,
                    weightedCriteriaRelation.Weights.ToList(),
                    c: c,
                    d: d);

                if (bfsCycleFinder.HasCycle(weightedCriteriaRelation.BestRelation))
                {
                    Console.WriteLine($"Для значення c = {string.Format("{0:0.000}", c)} у вiдношеннi для порогових значень 'c', 'd' був знайдений цикл, тому аналiз для поточного значення 'с' варто припинити");
                    continue;
                }

                removedCoreAlternatives = currentCore.Except(weightedCriteriaRelation.Core);
                addedCoreAlternatives = weightedCriteriaRelation.Core.Except(currentCore);
                if (removedCoreAlternatives.Count() != 0 || addedCoreAlternatives.Count() != 0)
                {
                    if (weightedCriteriaRelation.Core.Count != sizes.Last())
                        Console.WriteLine($"Для значення c = {string.Format("{0:0.000}", c)} розмiр ядра змiнився з {sizes.Last()} на {weightedCriteriaRelation.Core.Count}");
                    else
                        Console.WriteLine($"Для значення c = {string.Format("{0:0.000}", c)} розмiр ядра не змiнився, але змінилося його наповнення");
                    Console.WriteLine($"Попереднє наповнення ядра: {string.Join(' ', currentCore)}");
                    Console.WriteLine($"Поточне наповнення ядра: {string.Join(' ', weightedCriteriaRelation.Core)}");
                    if (removedCoreAlternatives.Count() != 0)
                        Console.WriteLine($"З ядра були виключенi значення: {string.Join(' ', removedCoreAlternatives)}");
                    if (addedCoreAlternatives.Count() != 0)
                        Console.WriteLine($"До ядра були доданi значення: {string.Join(' ', addedCoreAlternatives)}");
                    Console.WriteLine();
                }

                cValues.Add(c);
                sizes.Add(weightedCriteriaRelation.Core.Count);
                currentCore = weightedCriteriaRelation.Core;
            }

            string directoryPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string fileName = $"{directoryPath}\\sizes-c.txt";
            File.WriteAllText(fileName, string.Empty);
            File.WriteAllLines(fileName,
                new List<string>
                {
                    string.Join(' ', cValues.Select(elem => string.Format("{0:0.000}", elem).Replace(",", "."))),
                    string.Join(' ', sizes)
                });
        }

        public static void Task23(WeightedCriteriaRelation weightedCriteriaRelation)
        {
            double c = 1;
            weightedCriteriaRelation = new WeightedCriteriaRelation(
                    weightedCriteriaRelation.Evaluations,
                    weightedCriteriaRelation.Weights.ToList(),
                    c: c,
                    d: 0.001);

            Console.WriteLine($"Для значення c = 1, d = 0.001 розмiр ядра складає {weightedCriteriaRelation.Core.Count}");
            Console.WriteLine($"Поточне наповнення ядра: {string.Join(' ', weightedCriteriaRelation.Core)}");
            Console.WriteLine();

            var bfsCycleFinder = new BfsCycleFinder();
            List<double> cValues = new List<double>() { weightedCriteriaRelation.C };
            List<double> dValues = new List<double>() { weightedCriteriaRelation.D };
            List<int> sizes = new List<int>() { weightedCriteriaRelation.Core.Count };
            IReadOnlyCollection<int> currentCore = weightedCriteriaRelation.Core;
            IEnumerable<int> addedCoreAlternatives;
            IEnumerable<int> removedCoreAlternatives;
            for (double d = 0.002; d < 0.5; d = Math.Round(d + 0.001, 3, MidpointRounding.AwayFromZero))
            {
                c = Math.Round(c - 0.001, 3, MidpointRounding.AwayFromZero);

                weightedCriteriaRelation = new WeightedCriteriaRelation(
                    weightedCriteriaRelation.Evaluations,
                    weightedCriteriaRelation.Weights.ToList(),
                    c: c,
                    d: d);

                if (bfsCycleFinder.HasCycle(weightedCriteriaRelation.BestRelation))
                {
                    Console.WriteLine($"Для значень c = {string.Format("{0:0.000}", c)}, d = {string.Format("{0:0.000}", d)} у вiдношеннi для порогових значень 'c', 'd' був знайдений цикл, тому аналiз для поточного значення 'с' варто припинити");
                    continue;
                }

                removedCoreAlternatives = currentCore.Except(weightedCriteriaRelation.Core);
                addedCoreAlternatives = weightedCriteriaRelation.Core.Except(currentCore);
                if (removedCoreAlternatives.Count() != 0 || addedCoreAlternatives.Count() != 0)
                {
                    if (weightedCriteriaRelation.Core.Count != sizes.Last())
                        Console.WriteLine($"Для значень c = {string.Format("{0:0.000}", c)}, d = {string.Format("{0:0.000}", d)} розмiр ядра змiнився з {sizes.Last()} на {weightedCriteriaRelation.Core.Count}");
                    else
                        Console.WriteLine($"Для значень c = {string.Format("{0:0.000}", c)}, d = {string.Format("{0:0.000}", d)} розмiр ядра не змiнився, але змінилося його наповнення");
                    Console.WriteLine($"Попереднє наповнення ядра: {string.Join(' ', currentCore)}");
                    Console.WriteLine($"Поточне наповнення ядра: {string.Join(' ', weightedCriteriaRelation.Core)}");
                    if (removedCoreAlternatives.Count() != 0)
                        Console.WriteLine($"З ядра були виключенi значення: {string.Join(' ', removedCoreAlternatives)}");
                    if (addedCoreAlternatives.Count() != 0)
                        Console.WriteLine($"До ядра були доданi значення: {string.Join(' ', addedCoreAlternatives)}");
                    Console.WriteLine();
                }

                cValues.Add(c);
                dValues.Add(d);
                sizes.Add(weightedCriteriaRelation.Core.Count);
                currentCore = weightedCriteriaRelation.Core;
            }

            string directoryPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string fileName = $"{directoryPath}\\sizes-cd.txt";
            File.WriteAllText(fileName, string.Empty);
            File.WriteAllLines(fileName,
                new List<string>
                {
                    string.Join(' ', cValues.Select(elem => string.Format("{0:0.000}", elem).Replace(",", "."))),
                    string.Join(' ', dValues.Select(elem => string.Format("{0:0.000}", elem).Replace(",", "."))),
                    string.Join(' ', sizes)
                });
        }
    }
}
