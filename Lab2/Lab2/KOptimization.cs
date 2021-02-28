using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Lab2
{
    public class KOptimization
    {
        public HashSet<int> GetK1BestAlternatives(Relation relation)
        {
            return GetBestAlternatives(relation, new List<char> { 'I', 'P', 'N' });
        }

        public HashSet<int> GetK2BestAlternatives(Relation relation)
        {
            return GetBestAlternatives(relation, new List<char> { 'P', 'N' });
        }

        public HashSet<int> GetK3BestAlternatives(Relation relation)
        {
            return GetBestAlternatives(relation, new List<char> { 'I', 'P' });
        }

        public HashSet<int> GetK4BestAlternatives(Relation relation)
        {
            return GetBestAlternatives(relation, new List<char> { 'P' });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HashSet<int> GetBestAlternatives(
            Relation relation,
            List<char> suitableSetsMarkers)
        {
            int[][] characteristicSet = new int[relation.Dimension][];

            Dictionary<int, int> verticesRelationsCounter = new Dictionary<int, int>();
            for (int i = 0; i < relation.Dimension; i++)
            {
                characteristicSet[i] = new int[relation.Dimension];
                verticesRelationsCounter[i] = 0;
                for (int j = 0; j < relation.Dimension; j++)
                {
                    if (suitableSetsMarkers.Contains(relation.Characteristic[i][j]))
                    {
                        characteristicSet[i][j] = 1;
                        verticesRelationsCounter[i]++;
                    }
                }
            }

            Relation helperSetRelation = new Relation(characteristicSet);

            int maxForBetter = verticesRelationsCounter.Values.Max();
            HashSet<int> bestAlternatives =
                verticesRelationsCounter.Where(elem => elem.Value == maxForBetter)
                .Select(elem => elem.Key)
                .ToHashSet();

            Dictionary<int, HashSet<int>> bestAlternativesLowerSections =
                bestAlternatives.ToDictionary(elem => elem, elem => helperSetRelation.GetLowerSection(elem));

            HashSet<int> vertexLowerSection;
            for (int vertex = 0; vertex < helperSetRelation.Dimension; vertex++)
            {
                vertexLowerSection = helperSetRelation.GetLowerSection(vertex);
                foreach (var alternative in bestAlternativesLowerSections)
                {
                    if (vertexLowerSection.Except(alternative.Value).Count() > 0)
                        bestAlternatives.Remove(alternative.Key);
                }
            }

            return bestAlternatives;
        }
    }
}
