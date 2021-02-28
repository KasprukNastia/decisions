using System.Collections.Generic;

namespace Lab2
{
    public class BfsCycleFinder
    {
        public bool HasCycle(Relation relation)
        {
            Queue<int> queue;     
            int currentVertex;
            for(int vertex = 0; vertex < relation.Dimension; vertex++)
            {
                queue = new Queue<int>();

                for (int index = 0; index < relation.Dimension; index++)
                {
                    if (relation.Connections[vertex][index] == 1)
                        queue.Enqueue(index);
                }

                while (queue.Count > 0)
                {
                    currentVertex = queue.Dequeue();

                    if (currentVertex == vertex)
                        return true;

                    for (int index = 0; index < relation.Dimension; index++)
                    {
                        if (relation.Connections[currentVertex][index] == 1)
                            queue.Enqueue(index);
                    }
                }
            }

            return false;
        }
    }
}
