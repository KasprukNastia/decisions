﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab5
{
    public class VIKOR
    {
        /// <summary>
        /// Значення критеріїв для альтернатив
        /// </summary>
        public int[][] Evaluations { get; }

        public double VCoef { get; }

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

        private double[][] _normalizedEvaluations;
        public double[][] NormalizedEvaluations
        {
            get
            {
                if (_normalizedEvaluations == null)
                {
                   _normalizedEvaluations = new double[AlternativesCount][];

                    List<(int min, int max)> minMaxForCriterias = Enumerable.Range(0, CriteriasCount)
                        .Select(criteria => GetMinAndMaxForCriteria(criteria)).ToList();
                    for (int i = 0; i < AlternativesCount; i++)
                    {
                        _normalizedEvaluations[i] = new double[CriteriasCount];
                        for (int j = 0; j < CriteriasCount; j++)
                        {
                            (int min, int max) = minMaxForCriterias[j];
                            _normalizedEvaluations[i][j] = (max - Evaluations[i][j]) / (double)(max - min);
                        }
                    }
                }
                return _normalizedEvaluations;
            }
        }

        private double[][] _weightedNormalizedEvaluations;
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

        private List<(int alternative, double sValue)> _s;
        public List<(int alternative, double sValue)> S
        {
            get
            {
                if(_s == null)
                    _s = WeightedNormalizedEvaluations.Select((wne, alternative) => (alternative, wne.Sum()))
                        .ToList();
                return _s;
            }
        }

        private List<(int alternative, double rValue)> _r;
        public List<(int alternative, double rValue)> R
        {
            get
            {
                if (_r == null)
                    _r = WeightedNormalizedEvaluations.Select((wne, alternative) => (alternative, wne.Max()))
                        .ToList();
                return _r;
            }
        }

        private List<(int alternative, double qValue)> _q;
        public List<(int alternative, double qValue)> Q
        {
            get
            {
                if(_q == null)
                {
                    var sValues = S.Select(pair => pair.sValue);
                    var rValues = R.Select(pair => pair.rValue);
                    double sMin = sValues.Min(), sMax = sValues.Max(), rMin = rValues.Min(), rMax = rValues.Max();
                    _q = Enumerable.Range(0, AlternativesCount)
                        .Select(alternative => 
                            (alternative, (VCoef * (S[alternative].sValue - sMin) / (sMax - sMin)) + ((1 - VCoef) * (R[alternative].rValue - rMin) / (rMax - rMin))))
                        .ToList();
                }
                return _q;
            }
        }

        private List<int> _c1;
        public List<int> C1
        {
            get
            {
                if(_c1 == null)
                {
                    List<(int alternative, double qValue)> orderedQ = Q.OrderBy(elem => elem.qValue).ToList();

                    _c1 = new List<int>();
                    _c1.Add(orderedQ.First().alternative);
                    for(int i = 1; i < AlternativesCount; i++)
                    {
                        if ((orderedQ[i].qValue - orderedQ.First().qValue) < (1 / (double)(AlternativesCount - 1)))
                            _c1.Add(orderedQ[i].alternative);
                        else break;
                    }
                }
                return _c1;
            }
        }

        private List<int> _c2;
        public List<int> C2
        {
            get
            {
                if (_c2 == null)
                {
                    List<(int alternative, double sValue)> orderedS = S.OrderBy(elem => elem.sValue).ToList();
                    List<(int alternative, double rValue)> orderedR = R.OrderBy(elem => elem.rValue).ToList();

                    _c2 = new List<int>();
                    foreach(int alternative in C1)
                    {
                        if (S[alternative].sValue == orderedS.First().sValue ||
                            R[alternative].rValue == orderedR.First().rValue)
                            _c2.Add(alternative);
                    }
                }
                return _c2;
            }
        }

        private List<int> _finalResult;
        public List<int> FinalResult
        {
            get
            {
                if(_finalResult == null)
                    _finalResult = C1.Intersect(C2).ToList();
                return _finalResult;
            }
        }

        public VIKOR(int[][] evaluations,
            double vCoef,
            List<double> weights)
        {
            Evaluations = evaluations ?? throw new ArgumentNullException(nameof(evaluations));

            if (vCoef < 0 || vCoef > 1)
                throw new ArgumentException($"{nameof(vCoef)} value must be value between 0 and 1");
            VCoef = vCoef;

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
        }

        private (int min, int max) GetMinAndMaxForCriteria(int criteria)
        {
            int[] criteriaValues = new int[AlternativesCount];
            for (int i = 0; i < AlternativesCount; i++)
            {
                criteriaValues[i] = Evaluations[i][criteria];
            }

            return (criteriaValues.Min(), criteriaValues.Max());
        }
    }
}