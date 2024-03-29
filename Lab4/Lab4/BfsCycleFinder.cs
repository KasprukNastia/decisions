﻿using System.Collections.Generic;

namespace Lab4
{
    public class BfsCycleFinder
    {
        /// <summary>
        /// Пошук циклу у графі за допомогою алгоритму BFS
        /// </summary>
        public bool HasCycle(Relation relation)
        {
            for(int i = 0; i < relation.Dimension; i++)
            {
                for(int j = i; j < relation.Dimension; j++)
                {
                    if (relation.Characteristic[i][j].Equals('I'))
                        return true;
                }
            }

            // visited - множина для вже пройдених вершин, щоб не повертатися у них
            HashSet<int> visited = new HashSet<int>();
            // queue - черга для BFS
            Queue<int> queue;
            // currentVertex - номер вершини, витягнутої з черги BFS
            int currentVertex;
            // Проходимося по всім вершинам
            for (int vertex = 0; vertex < relation.Dimension; vertex++)
            {
                queue = new Queue<int>();

                // Додаємо всі сусідні для поточної (vertex) ще не пройдені вершини у чергу
                for (int index = 0; index < relation.Dimension; index++)
                {
                    if (relation.Connections[vertex][index] == 1 && !visited.Contains(index))
                        queue.Enqueue(index);
                }

                // Поки черга не спорожніє
                while (queue.Count > 0)
                {
                    currentVertex = queue.Dequeue();

                    // Якщо у черзі наткнулися на вершину, що співпадає з поточною, маємо цикл
                    if (currentVertex == vertex)
                        return true;

                    // Додаємо всі сусідні для витягнутої (currentVertex) ще не пройдені вершини у чергу
                    for (int index = 0; index < relation.Dimension; index++)
                    {
                        if (relation.Connections[currentVertex][index] == 1 && !visited.Contains(index))
                            queue.Enqueue(index);
                    }
                }

                // Додаємо поточну вершину у множену пройдених
                visited.Add(vertex);
            }

            return false;
        }
    }
}
