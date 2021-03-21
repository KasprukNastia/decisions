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
                    "матриця індексів узгодження C",
                    GetRelationString(weightedCriteriaRelation.CRelation),
                    "матриця індексів неузгодження D",
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
    }
}
