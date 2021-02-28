using System;
using System.Collections.Generic;

namespace Lab2
{
    public class Relation
    {
        public int[][] Connections { get; }
        public int Dimension { get; }


        private char[][] _characteristic;
        public char[][] Characteristic
        {
            get
            {
                if (_characteristic != null)
                    return _characteristic;

                _characteristic = new char[Dimension][];
                for(int i = 0; i < Dimension; i++)
                {
                    _characteristic[i] = new char[Dimension];
                    for(int j = i; j < Dimension; j++)
                    {
                        if(Connections[i][j] == 1 && Connections[j][i] == 1)
                            _characteristic[i][j] = _characteristic[j][i] = 'I';
                        else if(Connections[i][j] == 0 && Connections[j][i] == 0)
                            _characteristic[i][j] = _characteristic[j][i] = 'N';
                        else if (Connections[i][j] == 1 && Connections[j][i] == 0)
                            _characteristic[i][j] = 'P';
                        else if (Connections[i][j] == 0 && Connections[j][i] == 1)
                            _characteristic[j][i] = 'P';
                    }
                }

                return _characteristic;
            }
        }

        public Relation(int[][] connections)
        {
            Connections = connections ?? throw new ArgumentNullException(nameof(connections));

            Dimension = connections.Length;

            for(int i = 0; i < Dimension; i++)
            {
                if (connections[i].Length != Dimension)
                    throw new ArgumentException($"{nameof(connections)} must be represented as a square matrix");

                for (int j = 0; j < Dimension; j++)
                {
                    if (connections[i][j] != 0 && connections[i][j] != 1)
                        throw new ArgumentException($"{nameof(connections)} must be represented only as 0 or 1 digits");
                }
            }
        }

        public HashSet<int> GetUpperSection(int vertex) 
        {
            if (vertex < 0 || vertex >= Dimension)
                throw new ArgumentException($"The vertex {vertex} does not belong to the relation");

            HashSet<int> upperSection = new HashSet<int>();
            for (int i = 0; i < Dimension; i++)
            {
                if (Connections[i][vertex] == 1)
                    upperSection.Add(i);
            }

            return upperSection;
        }

        public HashSet<int> GetLowerSection(int vertex)
        {
            if (vertex < 0 || vertex >= Dimension)
                throw new ArgumentException($"The vertex {vertex} does not belong to the relation");

            HashSet<int> lowerSection = new HashSet<int>();
            for (int i = 0; i < Dimension; i++)
            {
                if (Connections[vertex][i] == 1)
                    lowerSection.Add(i);
            }

            return lowerSection;
        }
    }
}
