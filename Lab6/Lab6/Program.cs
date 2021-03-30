using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab6
{
    class Program
    {
        static void Main(string[] args)
        {
            int dimension = 6;
            double[][] relation1 = new double[][]
            {
                new double[] {0.3, 0.8, 1, 0, 0.5, 0.7},
                new double[] {0.1, 0, 0.3, 0, 0.7, 0.1},
                new double[] {0.1, 0.3, 0.8, 0.4, 0.3, 0.9},
                new double[] {0.5, 0.6, 0.1, 0.4, 0.4, 0.5},
                new double[] {0.2, 0.7, 0.5, 0.7, 0.3, 0.9},
                new double[] {0.7, 0.8, 0.5, 0.5, 0, 0.2},
            };
            double[][] relation2 = new double[][]
            {
                new double[] {0.1, 0.8, 0.5, 0.1, 0.8, 1},
                new double[] {0.7, 0.4, 0, 0.8, 0.4, 0.5},
                new double[] {0.7, 0.5, 0.8, 0.7, 0.2, 1},
                new double[] {0.6, 0.2, 0.7, 0.1, 0.8, 0.9},
                new double[] {0.7, 0.2, 1, 0.2, 0.7, 0.5},
                new double[] {0.7, 0.8, 0.5, 0.2, 0.2, 0.6 }
            };

            PrintEvaluations(dimension, dimension, () => Compose(relation1, relation2));
            Console.WriteLine();
            Console.WriteLine(IsTransitive(relation1));
        }

        public static double[][] Compose(double[][] relation1, double[][] relation2)
        {
            double[][] result = new double[relation1.Length][];
            for (int i = 0; i < relation1.Length; i++)
                result[i] = new double[relation1[i].Length];

            List<double> raw, column, minValues;
            for (int i = 0; i < relation1.Length; i++)
            {
                for (int j = 0; j < relation1.Length; j++)
                {
                    raw = relation1[i].ToList();
                    column = GetColumn(relation2, j).ToList();
                    minValues = raw.Select((elem, index) => Math.Min(elem, column[index])).ToList();

                    result[i][j] = minValues.Max();
                }
            }

            return result;

            double[] GetColumn(double[][] relation, int columnNumber)
            {
                double[] column = new double[relation.Length];
                for(int i = 0; i < relation.Length; i++)
                {
                    column[i] = relation[i][columnNumber];
                }
                return column;
            }
        }

        public static bool IsTransitive(double[][] relation)
        {
            for (int i = 0; i < relation.Length; i++)
            {
                for (int j = 0; j < relation.Length; j++)
                {
                    for(int k = 0; k < relation.Length; k++)
                    {
                        if (i == j && j == k)
                            continue;
                        if (relation[i][k] < Math.Min(relation[i][j], relation[j][k]))
                        {
                            Console.WriteLine($"[{i}][{k}] < min {{[{i}][{j}], [{j}][{k}]}}");
                            return false;
                        } 
                    }
                }
            }

            return true;
        }

        public static void PrintEvaluations(int rowsCount, int columnsCount, Func<double[][]> printingSelector)
        {
            double[][] toPrint = printingSelector();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"   {string.Join("   ", Enumerable.Range(0, columnsCount))}");
            for (int i = 0; i < rowsCount; i++)
            {
                Console.Write($"{i}{string.Concat(Enumerable.Repeat(' ', 3 - i.ToString().Length))}");
                Console.ForegroundColor = ConsoleColor.White;
                for (int j = 0; j < columnsCount; j++)
                {
                    Console.Write($"{string.Format("{0:0.0}", toPrint[i][j])}{string.Concat(Enumerable.Repeat(' ', (j + 1).ToString().Length))}");
                }
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }
    }
}
