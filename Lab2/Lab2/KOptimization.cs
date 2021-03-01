using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Lab2
{
    /// <summary>
    /// Клас, що реалізує алгоритм К-оптимізації
    /// </summary>
    public class KOptimization
    {
        // Маркери множин для К1-оптимізації
        private static readonly List<char> k1SuitableSetsMarkers = new List<char> { 'I', 'P', 'N' };
        // Маркери множин для К2-оптимізації
        private static readonly List<char> k2SuitableSetsMarkers = new List<char> { 'P', 'N' };
        // Маркери множин для К3-оптимізації
        private static readonly List<char> k3SuitableSetsMarkers = new List<char> { 'I', 'P' };
        // Маркери множин для К4-оптимізації
        private static readonly List<char> k4SuitableSetsMarkers = new List<char> { 'P' };

        /// <summary>
        /// Пошук К1-максимальних елементів
        /// </summary>
        public HashSet<int> GetK1BestAlternatives(Relation relation)
        {
            return GetBestAlternatives(relation, k1SuitableSetsMarkers);
        }

        /// <summary>
        /// Пошук К2-максимальних елементів
        /// </summary>
        public HashSet<int> GetK2BestAlternatives(Relation relation)
        {
            return GetBestAlternatives(relation, k2SuitableSetsMarkers);
        }

        /// <summary>
        /// Пошук К3-максимальних елементів
        /// </summary>
        public HashSet<int> GetK3BestAlternatives(Relation relation)
        {
            return GetBestAlternatives(relation, k3SuitableSetsMarkers);
        }

        /// <summary>
        /// Пошук К4-максимальних елементів
        /// </summary>
        public HashSet<int> GetK4BestAlternatives(Relation relation)
        {
            return GetBestAlternatives(relation, k4SuitableSetsMarkers);
        }

        /// <summary>
        /// Перевірка К1-оптимальних елементів
        /// </summary>
        public HashSet<int> CheckK1OptAlternatives(HashSet<int> k1BestAlternatives, Relation relation)
        {
            HashSet<int> optAlternatives = new HashSet<int>();

            // Перевіряємо всі К1-максимальні вершини
            foreach (int alternative in k1BestAlternatives)
            {
                // Якщо нижній переріз К1-максимальної вершини містить всі елементи відношення, то дана вершина є К1-оптимальною
                if (relation.Characteristic[alternative].Where(c => k1SuitableSetsMarkers.Contains(c)).Count() == relation.Dimension)
                    optAlternatives.Add(alternative);
            }

            return optAlternatives;
        }

        /// <summary>
        /// Реалізація алгоритму Кn-оптимізації
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HashSet<int> GetBestAlternatives(
            Relation relation,
            List<char> suitableSetsMarkers)
        {
            // Будуємо множину Sn
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

            // Відбираємо максимальні за включенням множини Sn
            int maxForBetter = verticesRelationsCounter.Values.Max();
            HashSet<int> bestAlternatives =
                verticesRelationsCounter.Where(elem => elem.Value == maxForBetter)
                .Select(elem => elem.Key)
                .ToHashSet();

            Dictionary<int, HashSet<int>> bestAlternativesLowerSections =
                bestAlternatives.ToDictionary(elem => elem, elem => helperSetRelation.GetLowerSection(elem));

            HashSet<int> vertexLowerSection;
            // Перевіряємо, що інші рядки не включають найкращі як власні підмножини 
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
