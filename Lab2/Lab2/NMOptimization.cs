using System.Collections.Generic;
using System.Linq;

namespace Lab2
{
    public class NMOptimization
    {
        public HashSet<int> GetBestAlternatives(Relation relation)
        {
            HashSet<int> allVertices = Enumerable.Range(0, relation.Dimension).ToHashSet();
            List<HashSet<int>> sSets = new List<HashSet<int>>();

            HashSet<int> prev = new HashSet<int>();
            HashSet<int> next;
            while (allVertices.Count > 0)
            {
                next = new HashSet<int>(prev);

                foreach (int vertex in allVertices)
                {
                    if (relation.GetUpperSection(vertex).Except(prev).Count() == 0)
                        next.Add(vertex);
                }

                sSets.Add(next);
                prev = next;

                allVertices = allVertices.Except(next).ToHashSet();
            }

            HashSet<int> bestAlternatives = new HashSet<int>(sSets[0]);
            if (sSets.Count == 1)
                return bestAlternatives;
            for(int i = 1; i < sSets.Count; i++)
            {
                next = sSets[i];
                foreach(int vertex in next)
                {
                    if (relation.GetUpperSection(vertex).Intersect(bestAlternatives).Count() == 0)
                        bestAlternatives.Add(vertex);
                }
            }

            return bestAlternatives;
        }

        public bool IsBestAlternativesInternallyStable(HashSet<int> bestAlternatives, Relation relation)
        {
            foreach(int alternative in bestAlternatives)
            {
                if (relation.GetUpperSection(alternative).Intersect(bestAlternatives).Count() > 0)
                    return false;
            }

            return true;
        }

        public bool IsBestAlternativesExternallyStable(HashSet<int> bestAlternatives, Relation relation)
        {
            HashSet<int> notBestAlternatives = 
                Enumerable.Range(0, relation.Dimension)
                .Except(bestAlternatives)
                .ToHashSet();

            foreach (int alternative in notBestAlternatives)
            {
                if (relation.GetUpperSection(alternative).Intersect(bestAlternatives).Count() == 0)
                    return false;
            }

            return true;
        }
    }
}
