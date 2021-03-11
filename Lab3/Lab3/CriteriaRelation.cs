using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3
{
    /// <summary>
    /// Клас, що описує значення критеріїв для альтернатив
    /// </summary>
    public class CriteriaRelation
    {
        /// <summary>
        /// Значення критеріїв для альтернатив
        /// </summary>
        public int[][] Evaluations { get; }

        /// <summary>
        /// Упорядкована за спаданням важливості множина критеріїв
        /// </summary>
        public IReadOnlyCollection<int> CriteriasImportance { get; }

        /// <summary>
        /// Класи впорядковані за зростанням важливості
        /// </summary>
        public IReadOnlyCollection<IReadOnlyCollection<int>> CriteriasImportancesClasses { get; }

        /// <summary>
        /// К-сть критеріїв
        /// </summary>
        public int CriteriasCount { get; }

        /// <summary>
        /// К-сть альтернатив
        /// </summary>
        public int AlternativesCount { get; }

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

        /// <summary>
        /// Матриця сигма векторів
        /// </summary>
        private List<int>[][] _sigmaVectors;
        /// <summary>
        /// Матриця сигма векторів
        /// </summary>
        public List<int>[][] SigmaVectors
        {
            get
            {
                if(_sigmaVectors != null)
                    return _sigmaVectors;

                _sigmaVectors = new List<int>[AlternativesCount][];
                for (int i = 0; i < AlternativesCount; i++)
                    _sigmaVectors[i] = new List<int>[AlternativesCount];

                for (int i = 0; i < AlternativesCount; i++)
                {
                    _sigmaVectors[i][i] = Enumerable.Repeat(0, CriteriasCount).ToList();

                    for (int j = i + 1; j < AlternativesCount; j++)
                    {
                        _sigmaVectors[i][j] = DeltaVectors[i][j].Select(elem => elem > 0 ? 1 : elem == 0 ? 0 : -1).ToList();
                        _sigmaVectors[j][i] = _sigmaVectors[i][j].Select(elem => elem * -1).ToList();
                    }
                }

                return _sigmaVectors;
            }
        }

        /// <summary>
        /// Значення критеріїв для альтернатив, упорядковані за спаданням важливості критеріїв
        /// </summary>
        private CriteriaRelation _sortedCriteriaRelation;
        /// <summary>
        /// Значення критеріїв для альтернатив, упорядковані за спаданням важливості критеріїв
        /// </summary>
        public CriteriaRelation SortedCriteriaRelation
        {
            get
            {
                if (_sortedCriteriaRelation != null)
                    return _sortedCriteriaRelation;

                int[][] sortedEvaluations = new int[AlternativesCount][];

                for (int i = 0; i < AlternativesCount; i++)
                    sortedEvaluations[i] = new int[CriteriasCount];

                int counter = 0;
                foreach (int criteria in CriteriasImportance)
                {
                    for (int j = 0; j < AlternativesCount; j++)
                        sortedEvaluations[j][counter] = Evaluations[j][criteria];
                    counter++;
                }

                _sortedCriteriaRelation = new CriteriaRelation(sortedEvaluations);
                return _sortedCriteriaRelation;
            }
        }

        /// <summary>
        /// Відношення Парето
        /// </summary>
        private Relation _paretoRelation;
        /// <summary>
        /// Відношення Парето
        /// </summary>
        public Relation ParetoRelation
        {
            get
            {
                if (_paretoRelation != null)
                    return _paretoRelation;

                int[][] paretoRelation = new int[AlternativesCount][];

                for (int i = 0; i < AlternativesCount; i++)
                    paretoRelation[i] = new int[AlternativesCount];

                for(int i = 0; i < AlternativesCount; i++)
                {
                    for(int j = 0; j < AlternativesCount; j++)
                    {
                        // альтернатива i переважає j, якщо сигма вектор пари (i,j) не містить значень -1
                        if (SigmaVectors[i][j].Any(elem => elem == -1))
                            paretoRelation[i][j] = 0;
                        else
                            paretoRelation[i][j] = 1;
                    }
                }

                _paretoRelation = new Relation(paretoRelation);
                return _paretoRelation;
            }
        }

        /// <summary>
        /// Мажоритарне відношення
        /// </summary>
        private Relation _majorityRelation;
        /// <summary>
        /// Мажоритарне відношення
        /// </summary>
        public Relation MajorityRelation
        {
            get
            {
                if (_majorityRelation != null)
                    return _majorityRelation;

                int[][] majorityRelation = new int[AlternativesCount][];

                for (int i = 0; i < AlternativesCount; i++)
                    majorityRelation[i] = new int[AlternativesCount];

                for (int i = 0; i < AlternativesCount; i++)
                {
                    for (int j = 0; j < AlternativesCount; j++)
                    {
                        // альтернатива i переважає j, якщо сума елементів вектору сигма більша нуля
                        if (SigmaVectors[i][j].Sum() > 0)
                            majorityRelation[i][j] = 1;
                        else
                            majorityRelation[i][j] = 0;
                    }
                }

                _majorityRelation = new Relation(majorityRelation);
                return _majorityRelation;
            }
        }

        /// <summary>
        /// Лексикографічне відношення
        /// </summary>
        private Relation _lexicographicRelation;
        /// <summary>
        /// Лексикографічне відношення
        /// </summary>
        public Relation LexicographicRelation
        {
            get
            {
                if (_lexicographicRelation != null)
                    return _lexicographicRelation;

                int[][] lexicographicRelation = new int[AlternativesCount][];

                for (int i = 0; i < AlternativesCount; i++)
                    lexicographicRelation[i] = new int[AlternativesCount];

                for (int i = 0; i < AlternativesCount; i++)
                {
                    for (int j = 0; j < AlternativesCount; j++)
                    {
                        foreach(int elem in SortedCriteriaRelation.SigmaVectors[i][j])
                        {
                            // альтернатива i переважає j, якщо сигма вектор має на своєму початку 
                            // будь-яку кількість нулів, а потім одиницю
                            if (elem == 0)
                                continue;
                            if(elem == 1)
                                lexicographicRelation[i][j] = 1;
                            else
                                lexicographicRelation[i][j] = 0;
                            break;
                        }
                    }
                }

                _lexicographicRelation = new Relation(lexicographicRelation);
                return _lexicographicRelation;
            }
        }

        /// <summary>
        /// Відношення Березовського
        /// </summary>
        private Relation _BerezovskyRelation;
        /// <summary>
        /// Відношення Березовського
        /// </summary>
        public Relation BerezovskyRelation
        {
            get
            {
                if (_BerezovskyRelation != null || CriteriasImportancesClasses.Count == 0)
                    return _BerezovskyRelation;

                List<CriteriaRelation> criteriaRelationsByClasses = 
                    new List<CriteriaRelation>(CriteriasImportancesClasses.Count);

                int[][] sortedEvaluations;
                int counter;
                // Формування CriteriaRelation для кожного з класів CriteriasImportancesClasses
                foreach (IReadOnlyCollection<int> criteriaClass in CriteriasImportancesClasses)
                {
                    sortedEvaluations = new int[AlternativesCount][];
                    for (int i = 0; i < AlternativesCount; i++)
                        sortedEvaluations[i] = new int[criteriaClass.Count];

                    counter = 0;
                    foreach (int criteria in criteriaClass)
                    {
                        for (int j = 0; j < AlternativesCount; j++)
                            sortedEvaluations[j][counter] = Evaluations[j][criteria];
                        counter++;
                    }
                    criteriaRelationsByClasses.Add(new CriteriaRelation(sortedEvaluations));
                }

                Relation currentBerezovskyRelation = criteriaRelationsByClasses.First().ParetoRelation;
                List<char> possibleCharacteristics = new List<char> { 'P', 'N', 'I' };
                Relation currentClassParetoRelation;
                int[][] nextBerezovskyRelation;
                // Ітераційний процес для формування відношення Березовського
                for (int criteriaClass = 1; criteriaClass < criteriaRelationsByClasses.Count; criteriaClass++)
                {
                    currentClassParetoRelation = criteriaRelationsByClasses[criteriaClass].ParetoRelation;
                    nextBerezovskyRelation = new int[AlternativesCount][];
                    for (int i = 0; i < AlternativesCount; i++)
                        nextBerezovskyRelation[i] = new int[AlternativesCount];

                    for (int i = 0; i < AlternativesCount; i++)
                    {
                        for (int j = 0; j < AlternativesCount; j++)
                        {
                            if ((currentClassParetoRelation.Characteristic[i][j].Equals('P') &&
                                possibleCharacteristics.Any(c => c.Equals(currentBerezovskyRelation.Characteristic[i][j]))) ||
                                (currentClassParetoRelation.Characteristic[i][j].Equals('I') &&
                                currentBerezovskyRelation.Characteristic[i][j].Equals('P')))
                            {
                                nextBerezovskyRelation[i][j] = 1;
                                nextBerezovskyRelation[j][i] = 0;
                            }
                            else if (currentClassParetoRelation.Characteristic[i][j].Equals('I') &&
                                currentBerezovskyRelation.Characteristic[i][j].Equals('I') &&
                                criteriaClass != criteriaRelationsByClasses.Count - 1)
                                nextBerezovskyRelation[i][j] = nextBerezovskyRelation[j][i] = 1;
                            else
                                nextBerezovskyRelation[i][j] = 0;
                        }
                    }

                    currentBerezovskyRelation = new Relation(nextBerezovskyRelation);
                }

                _BerezovskyRelation = currentBerezovskyRelation;
                return _BerezovskyRelation;
            }
        }

        /// <summary>
        /// Відношення Подиновського
        /// </summary>
        private Relation _PodinovskyRelation;
        /// <summary>
        /// Відношення Подиновського
        /// </summary>
        public Relation PodinovskyRelation
        {
            get
            {
                if (_PodinovskyRelation != null)
                    return _PodinovskyRelation;

                // Сортування значень критеріїв для кожної з альтернатив
                int[][] sortedEvaluations = new int[AlternativesCount][];
                for (int i = 0; i < AlternativesCount; i++)
                    sortedEvaluations[i] = Evaluations[i].OrderByDescending(e => e).ToArray();

                var podinovskyCriteriaRelation = new CriteriaRelation(sortedEvaluations);

                // Отримання відношення Парето для відсортованих критеріїв
                _PodinovskyRelation = podinovskyCriteriaRelation.ParetoRelation;
                return _PodinovskyRelation;
            }
        }

        public CriteriaRelation(int[][] evaluations, 
            HashSet<int> criteriasImportance = null,
            List<HashSet<int>> criteriasImportancesClasses = null)
        {
            Evaluations = evaluations ?? throw new ArgumentNullException(nameof(evaluations));

            AlternativesCount = evaluations.Length;

            if (AlternativesCount > 0)
                CriteriasCount = evaluations[0].Length;

            if (criteriasImportance == null)
                criteriasImportance = Enumerable.Range(0, CriteriasCount).ToHashSet();
            else
            {
                CriteriasImportance = criteriasImportance;
                if (criteriasImportance.Count != CriteriasCount)
                    throw new ArgumentException($"The number of criterias does not match");
                if (CriteriasImportance.Any(elem => elem < 0 || elem >= CriteriasImportance.Count))
                    throw new ArgumentException($"{nameof(criteriasImportance)} contains not existing criteria");
            }

            CriteriasImportancesClasses = criteriasImportancesClasses;

            for (int i = 1; i < AlternativesCount; i++)
            {
                if (evaluations[i].Length != CriteriasCount)
                    throw new ArgumentException($"Evaluation must be provided only for {CriteriasCount} criterias");
            }
        }
    }
}
