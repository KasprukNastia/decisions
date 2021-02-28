﻿using System;
using System.Collections.Generic;

namespace Lab2
{
    public class Relation
    {
        public int[][] Connections { get; }

        public int Dimension { get; }
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
    }
}