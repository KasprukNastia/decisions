using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab4
{
    public class WeightedCriteriaRelation
    {
        /// <summary>
        /// Значення критеріїв для альтернатив
        /// </summary>
        public int[][] Evaluations { get; }

        public IReadOnlyList<double> Weights { get; }

        /// <summary>
        /// К-сть критеріїв
        /// </summary>
        public int CriteriasCount { get; }

        /// <summary>
        /// К-сть альтернатив
        /// </summary>
        public int AlternativesCount { get; }

        public double C { get; }
        
        public double D { get; }

        /// <summary>
        /// Матриця дельта векторів
        /// </summary>
        private List<int>[][] _deltaVectors;
        /// <summary>
        /// Матриця дельта векторів
        /// </summary>
        public List<int>[][] DeltaVectors
        {
            get
            {
                if (_deltaVectors != null)
                    return _deltaVectors;

                _deltaVectors = new List<int>[AlternativesCount][];
                for (int i = 0; i < AlternativesCount; i++)
                    _deltaVectors[i] = new List<int>[AlternativesCount];

                for (int i = 0; i < AlternativesCount; i++)
                {
                    _deltaVectors[i][i] = Enumerable.Repeat(0, AlternativesCount).ToList();

                    for (int j = i + 1; j < AlternativesCount; j++)
                    {
                        _deltaVectors[i][j] = Evaluations[i].Select((elem, index) => elem - Evaluations[j][index]).ToList();
                        _deltaVectors[j][i] = _deltaVectors[i][j].Select(elem => elem * -1).ToList();
                    }
                }

                return _deltaVectors;
            }
        }

        private double[][] _cRelation;
        public double[][] CRelation
        {
            get
            {
                if (_cRelation != null)
                    return _cRelation;

                _cRelation = new double[AlternativesCount][];
                for (int i = 0; i < AlternativesCount; i++)
                    _cRelation[i] = new double[AlternativesCount];

                double weidhtsSum = Weights.Sum();
                for (int i = 0; i < AlternativesCount; i++)
                {
                    _cRelation[i][i] = 0;

                    for (int j = i + 1; j < AlternativesCount; j++)
                    {
                        _cRelation[i][j] = 
                            Math.Round(DeltaVectors[i][j].Select((elem, index) => elem >= 0 ? Weights[index] : 0).Sum() / weidhtsSum, 3, MidpointRounding.AwayFromZero);
                        _cRelation[j][i] =
                            Math.Round(DeltaVectors[j][i].Select((elem, index) => elem >= 0 ? Weights[index] : 0).Sum() / weidhtsSum, 3, MidpointRounding.AwayFromZero);
                    }
                }

                return _cRelation;
            }
        }

        private double[][] _dRelation;
        public double[][] DRelation
        {
            get
            {
                if (_dRelation != null)
                    return _dRelation;

                _dRelation = new double[AlternativesCount][];
                for (int i = 0; i < AlternativesCount; i++)
                    _dRelation[i] = new double[AlternativesCount];

                for (int i = 0; i < AlternativesCount; i++)
                {
                    _dRelation[i][i] = 1;

                    for (int j = i + 1; j < AlternativesCount; j++)
                    {
                        _dRelation[i][j] = GetDValue(DeltaVectors[i][j]);
                        _dRelation[j][i] = GetDValue(DeltaVectors[j][i]);
                    }
                }

                return _dRelation;
            }
        }

        private Relation _bestRelation;
        public Relation BestRelation
        {
            get
            {
                if (_bestRelation != null)
                    return _bestRelation;

                int[][] bestRelation = new int[AlternativesCount][];
                for (int i = 0; i < AlternativesCount; i++)
                    bestRelation[i] = new int[AlternativesCount];

                for (int i = 0; i < AlternativesCount; i++)
                {
                    for (int j = i; j < AlternativesCount; j++)
                    {
                        bestRelation[i][j] = CRelation[i][j] >= C && DRelation[i][j] <= D ? 1 : 0;
                        bestRelation[j][i] = CRelation[j][i] >= C && DRelation[j][i] <= D ? 1 : 0;
                    }
                }

                _bestRelation = new Relation(bestRelation);

                return _bestRelation;
            }
        }

        private HashSet<int> _core;
        public IReadOnlyCollection<int> Core
        {
            get
            {
                if (_core != null)
                    return _core;

                var optimizator = new NMOptimization();
                _core = optimizator.GetBestAlternatives(BestRelation).OrderBy(e => e).ToHashSet();

                if (!optimizator.IsBestAlternativesInternallyStable(_core, BestRelation) ||
                    !optimizator.IsBestAlternativesExternallyStable(_core, BestRelation))
                    throw new InvalidOperationException("Error in core calculation");

                return _core;
            }
        }

        public WeightedCriteriaRelation(int[][] evaluations,
            List<double> weights,
            double c,
            double d)
        {
            Evaluations = evaluations ?? throw new ArgumentNullException(nameof(evaluations));

            AlternativesCount = evaluations.Length;
            if (AlternativesCount > 0)
                CriteriasCount = evaluations[0].Length;
            for (int i = 1; i < AlternativesCount; i++)
            {
                if (evaluations[i].Length != CriteriasCount)
                    throw new ArgumentException($"Evaluation must be provided only for {CriteriasCount} criterias");
            }

            Weights = weights ?? throw new ArgumentNullException(nameof(weights));
            if (Weights.Count != CriteriasCount)
                throw new ArgumentException("The number of weights does not meet the number of criterias");

            if (c < 0 || c > 1)
                throw new ArgumentException("Value of 'c' must be between 0 and 1");
            C = c;

            if (d < 0 || d > 1)
                throw new ArgumentException("Value of 'd' must be between 0 and 1");
            D = d;
        }

        private double GetDValue(List<int> deltaVector)
        {
            var indices = new List<int>();
            double nominator = deltaVector.Select((elem, index) =>
            {
                if (elem < 0)
                {
                    indices.Add(index);
                    return Weights[index] * -1 * elem;
                }
                return 0;
            }).Max();
            double denominator = indices.Count > 0 ?
                indices.Select(index => Weights[index] * GetDeltaForCriteria(index)).Max() :
                1;

            return Math.Round(nominator / denominator, 3, MidpointRounding.AwayFromZero);
        }

        private int GetDeltaForCriteria(int criteria)
        {
            int[] criteriaValues = new int[AlternativesCount];
            for (int i = 0; i < AlternativesCount; i++)
            {
                criteriaValues[i] = Evaluations[i][criteria];
            }

            return criteriaValues.Max() - criteriaValues.Min();
        }
    }
}
