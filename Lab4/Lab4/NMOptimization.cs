using System.Collections.Generic;
using System.Linq;

namespace Lab4
{
    /// <summary>
    /// Клас, що реалізує алгоритм Неймана-Моргенштерна
    /// </summary>
    public class NMOptimization
    {
        /// <summary>
        /// Пошук найкращих альтернатив за алгоритмом Неймана-Моргенштерна
        /// </summary>
        public HashSet<int> GetBestAlternatives(Relation relation)
        {
            HashSet<int> allVertices = Enumerable.Range(0, relation.Dimension).ToHashSet();
            List<HashSet<int>> sSets = new List<HashSet<int>>();

            HashSet<int> prev = new HashSet<int>();
            HashSet<int> next;
            // Побудова множин Sn
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
            // Побудова множин Qn
            for (int i = 1; i < sSets.Count; i++)
            {
                next = sSets[i];
                foreach (int vertex in next)
                {
                    if (relation.GetUpperSection(vertex).Intersect(bestAlternatives).Count() == 0)
                        bestAlternatives.Add(vertex);
                }
            }

            return bestAlternatives;
        }

        /// <summary>
        /// Перевірка найкращих альтернатив на внутрішню стійкість
        /// </summary>
        public bool IsBestAlternativesInternallyStable(HashSet<int> bestAlternatives, Relation relation)
        {
            // Якщо верхній переріз якої-небудь з найкращих альтернатив містить 
            // у собі іншу найкращу альтернативу, то множина розв'язків не є внутрішньо стійкою
            foreach (int alternative in bestAlternatives)
            {
                if (relation.GetUpperSection(alternative).Intersect(bestAlternatives).Count() > 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Перевірка найкращих альтернатив на зовнішню стійкість
        /// </summary>
        public bool IsBestAlternativesExternallyStable(HashSet<int> bestAlternatives, Relation relation)
        {
            HashSet<int> notBestAlternatives =
                Enumerable.Range(0, relation.Dimension)
                .Except(bestAlternatives)
                .ToHashSet();

            // Якщо верхній переріз якої-небудь з альтернатив, що не входять до найкращих, не містить 
            // у собі жодної з найкращих альтернатив, то множина розв'язків не є зовнішньо стійкою
            foreach (int alternative in notBestAlternatives)
            {
                if (relation.GetUpperSection(alternative).Intersect(bestAlternatives).Count() == 0)
                    return false;
            }

            return true;
        }
    }
}
