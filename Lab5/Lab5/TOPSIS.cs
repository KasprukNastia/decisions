﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab5
{
    /// <summary>
    /// Клас, що описує метод TOPSIS
    /// </summary>
    public class TOPSIS
    {
        /// <summary>
        /// Значення критеріїв для альтернатив
        /// </summary>
        public int[][] Evaluations { get; }

        /// <summary>
        /// Ваги критеріїв
        /// </summary>
        public IReadOnlyList<double> Weights { get; }

        /// <summary>
        /// Критерії, які потрібно максимізувати
        /// </summary>
        public IReadOnlyList<int> WeightsToMaximize { get; }

        /// <summary>
        /// Критерії, які потрібно мінімізувати
        /// </summary>
        public IReadOnlyList<int> WeightsToMinimize { get; }

        /// <summary>
        /// К-сть критеріїв
        /// </summary>
        public int CriteriasCount { get; }

        /// <summary>
        /// К-сть альтернатив
        /// </summary>
        public int AlternativesCount { get; }

        /// <summary>
        /// Нормалізовані оцінки альтернатив
        /// </summary>
        private double[][] _normalizedEvaluations;
        /// <summary>
        /// Нормалізовані оцінки альтернатив
        /// </summary>
        public double[][] NormalizedEvaluations
        {
            get
            {
                if (_normalizedEvaluations == null)
                {
                    if (WeightsToMaximize == null && WeightsToMinimize == null)
                        _normalizedEvaluations = CalcNormalizedAllMax();
                    else
                        _normalizedEvaluations = CalcNormalizedMinAndMax();
                }
                return _normalizedEvaluations;
            }
        }

        /// <summary>
        /// Зважені нормалізовані оцінки альтернатив
        /// </summary>
        private double[][] _weightedNormalizedEvaluations;
        /// <summary>
        /// Зважені нормалізовані оцінки альтернатив
        /// </summary>
        public double[][] WeightedNormalizedEvaluations
        {
            get
            {
                if (_weightedNormalizedEvaluations == null)
                {
                    _weightedNormalizedEvaluations = new double[AlternativesCount][];
                    for (int i = 0; i < AlternativesCount; i++)
                    {
                        _weightedNormalizedEvaluations[i] = new double[CriteriasCount];
                        for (int j = 0; j < CriteriasCount; j++)
                        {
                            _weightedNormalizedEvaluations[i][j] = NormalizedEvaluations[i][j] * Weights[j];
                        }
                    }
                }
                return _weightedNormalizedEvaluations;
            }
        }

        /// <summary>
        /// Утопічна точка
        /// </summary>
        private IReadOnlyList<double> _pis;
        /// <summary>
        /// Утопічна точка
        /// </summary>
        public IReadOnlyList<double> PIS
        {
            get
            {
                if (_pis == null)
                {
                    if (WeightsToMaximize == null && WeightsToMinimize == null)
                        _pis = Enumerable.Range(0, CriteriasCount)
                            .Select(criteria => GetMinAndMaxForCriteriaInWeighted(criteria))
                            .Select(minMax => minMax.max).ToList();
                    else
                        _pis = Enumerable.Range(0, CriteriasCount)
                            .Select(criteria => GetMinAndMaxForCriteriaInWeighted(criteria))
                            .Select((minMax, criteria) => WeightsToMaximize.Contains(criteria) ? minMax.max : minMax.min)
                            .ToList();
                }
                return _pis;
            }
        }

        /// <summary>
        /// Антиутопічна точка
        /// </summary>
        private IReadOnlyList<double> _nis;
        /// <summary>
        /// Антиутопічна точка
        /// </summary>
        public IReadOnlyList<double> NIS
        {
            get
            {
                if(_nis == null)
                {
                    if (WeightsToMaximize == null && WeightsToMinimize == null)
                        _nis = Enumerable.Range(0, CriteriasCount)
                            .Select(criteria => GetMinAndMaxForCriteriaInWeighted(criteria))
                            .Select(minMax => minMax.min).ToList();
                    else
                        _nis = Enumerable.Range(0, CriteriasCount)
                            .Select(criteria => GetMinAndMaxForCriteriaInWeighted(criteria))
                            .Select((minMax, criteria) => WeightsToMaximize.Contains(criteria) ? minMax.min : minMax.max)
                            .ToList();
                }
                return _nis;
            }
        }

        /// <summary>
        /// Відстані до утопічної точки
        /// </summary>
        private IReadOnlyList<double> _dToPis;
        /// <summary>
        /// Відстані до утопічної точки
        /// </summary>
        public IReadOnlyList<double> DToPIS
        {
            get
            {
                if(_dToPis == null)
                {
                    double[] dToPis = new double[AlternativesCount];
                    for(int alternative = 0; alternative < AlternativesCount; alternative++)
                    {
                        dToPis[alternative] = Math.Sqrt(
                             WeightedNormalizedEvaluations[alternative]
                                .Select((elem, criteria) => Math.Pow(elem - PIS[criteria], 2))
                                .Sum());
                    }
                    _dToPis = dToPis.ToList();
                }
                return _dToPis;
            }
        }

        /// <summary>
        /// Відстані до антиутопічної точки
        /// </summary>
        private IReadOnlyList<double> _dToNis;
        /// <summary>
        /// Відстані до антиутопічної точки
        /// </summary>
        public IReadOnlyList<double> DToNIS
        {
            get
            {
                if (_dToNis == null)
                {
                    double[] dToNis = new double[AlternativesCount];
                    for (int alternative = 0; alternative < AlternativesCount; alternative++)
                    {
                        dToNis[alternative] = Math.Sqrt(
                             WeightedNormalizedEvaluations[alternative]
                                .Select((elem, criteria) => Math.Pow(elem - NIS[criteria], 2))
                                .Sum());
                    }
                    _dToNis = dToNis.ToList();
                }
                return _dToNis;
            }
        }

        /// <summary>
        /// Наближеності до утопічної точки
        /// </summary>
        private IReadOnlyList<(int alternative, double closennes)> _c;
        /// <summary>
        /// Наближеності до утопічної точки
        /// </summary>
        public IReadOnlyList<(int alternative, double closennes)> C
        {
            get
            {
                if(_c == null)
                    _c = DToNIS.Select((dToNis, alternative) => (alternative, dToNis / (DToPIS[alternative] + dToNis)))
                        .OrderByDescending(elem => elem.Item2)
                        .ToList();
                return _c;
            }
        }

        public TOPSIS(int[][] evaluations,
            IReadOnlyList<double> weights,
            IReadOnlyList<int> weightsToMaximize = null,
            IReadOnlyList<int> weightsToMinimize = null)
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
            double weightsSum = Weights.Sum();
            Weights = Weights.Select(w => w / weightsSum).ToList();

            WeightsToMaximize = weightsToMaximize;
            WeightsToMinimize = weightsToMinimize;
        }

        /// <summary>
        /// Обчислення нормалізованих оцінок альтернатив для випадку, коли всі критерії потрібно максимізувати
        /// </summary>
        private double[][] CalcNormalizedAllMax()
        {
            double[][] normalized = new double[AlternativesCount][];

            List<double> geomMeansForCriterias = Enumerable.Range(0, CriteriasCount)
                .Select(criteria => CalcGeometricMeanForCriteria(criteria)).ToList();
            for (int i = 0; i < AlternativesCount; i++)
            {
                normalized[i] = new double[CriteriasCount];
                for(int j = 0; j < CriteriasCount; j++)
                {
                    normalized[i][j] = Evaluations[i][j] / geomMeansForCriterias[j];
                }
            }

            return normalized;
        }

        /// <summary>
        /// Обчислення нормалізованих оцінок альтернатив для випадку, 
        /// коли частину критеріїв потрібно максимізувати, а частину мінімізувати
        /// </summary>
        private double[][] CalcNormalizedMinAndMax()
        {
            double[][] normalized = new double[AlternativesCount][];

            List<(int min, int max)> minMaxForCriterias = Enumerable.Range(0, CriteriasCount)
                .Select(criteria => GetMinAndMaxForCriteria(criteria)).ToList();
            for (int i = 0; i < AlternativesCount; i++)
            {
                normalized[i] = new double[CriteriasCount];
                for (int j = 0; j < CriteriasCount; j++)
                {
                    (int min, int max) = minMaxForCriterias[j];
                    normalized[i][j] = WeightsToMaximize.Contains(j) ?
                        (Evaluations[i][j] - min) / (double)(max - min) :
                        (max - Evaluations[i][j]) / (double)(max - min);
                }
            }

            return normalized;
        }

        /// <summary>
        /// Обчислення середнього геометричного для значень критерію
        /// </summary>
        private double CalcGeometricMeanForCriteria(int criteria)
        {
            double sum = 0;
            for(int alternative = 0; alternative < AlternativesCount; alternative++)
            {
                sum += Math.Pow(Evaluations[alternative][criteria], 2);
            }
            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Обчислення мінімального та максимально значень для критерію
        /// з початкових оцінок альтернатив
        /// </summary>
        private (int min, int max) GetMinAndMaxForCriteria(int criteria)
        {
            int[] criteriaValues = new int[AlternativesCount];
            for (int i = 0; i < AlternativesCount; i++)
            {
                criteriaValues[i] = Evaluations[i][criteria];
            }

            return (criteriaValues.Min(), criteriaValues.Max());
        }

        /// <summary>
        /// Обчислення мінімального та максимально значень для критерію
        /// зі зважених оцінок альтернатив
        /// </summary>
        private (double min, double max) GetMinAndMaxForCriteriaInWeighted(int criteria)
        {
            double[] criteriaValues = new double[AlternativesCount];
            for (int i = 0; i < AlternativesCount; i++)
            {
                criteriaValues[i] = WeightedNormalizedEvaluations[i][criteria];
            }

            return (criteriaValues.Min(), criteriaValues.Max());
        }
    }
}
