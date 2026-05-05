using PathfindingVisualizer.Client.Models;

namespace PathfindingVisualizer.Client.Algorithms
{
    public class BfsSolver
    {
        public async Task<(bool pathFound, int visitedNodes)> SolveAsync(Node[,] grid, Node start, Node finish, Action updateUI, int delayMs, bool allowDiagonals)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            int visitedNodes = 0;

            var queue = new Queue<Node>();
            queue.Enqueue(start);

            int[] dRow = allowDiagonals
                ? new int[] { -1, -1, 0, 1, 1, 1, 0, -1 }
                : new int[] { -1, 0, 1, 0 };
            int[] dCol = allowDiagonals
                ? new int[] { 0, 1, 1, 1, 0, -1, -1, -1 }
                : new int[] { 0, 1, 0, -1 };

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == finish) return (true, visitedNodes);

                for (int i = 0; i < dRow.Length; i++)
                {
                    int newRow = current.Row + dRow[i];
                    int newCol = current.Col + dCol[i];

                    if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
                    {
                        var neighbor = grid[newRow, newCol];

                        if (neighbor.Status == NodeStatus.Empty || neighbor.Status == NodeStatus.Finish)
                        {
                            if (neighbor != finish)
                            {
                                neighbor.Status = NodeStatus.Visited;
                                visitedNodes++;
                            }

                            neighbor.Parent = current;
                            queue.Enqueue(neighbor);
                        }
                    }
                }
                updateUI.Invoke();
                await Task.Delay(delayMs);
            }
            return (false, visitedNodes);
        }

        public async Task<int> DrawPathAsync(Node finish, Node start, Action updateUI, int delayMs)
        {
            int pathLength = 0;
            var current = finish.Parent;

            while (current != null && current != start)
            {
                current.Status = NodeStatus.Path;
                pathLength++;
                updateUI.Invoke();
                await Task.Delay(delayMs);
                current = current.Parent;
            }
            return pathLength + 1;
        }
    }
}