using System;

namespace Lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            var relation = new Relation(
                new int[][]
                {
                    new int[] {0, 0, 1, 1, 0, 1},
                    new int[] {1, 0, 1, 1, 1, 1},
                    new int[] {0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 0, 0, 0, 0},
                    new int[] {0, 0, 1, 1, 0, 1},
                    new int[] {0, 0, 0, 0, 0, 0}
                });

            var nm = new NMOptimization();
            var bfsCycleFinder = new BfsCycleFinder();
            //var res = nm.GetBestAlternatives(relation);

            bool hasCycle = bfsCycleFinder.HasCycle(relation);
            Console.WriteLine(hasCycle);
        }
    }
}
