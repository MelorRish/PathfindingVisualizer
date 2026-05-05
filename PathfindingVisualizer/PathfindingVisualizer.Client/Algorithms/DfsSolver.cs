using PathfindingVisualizer.Client.Models;

namespace PathfindingVisualizer.Client.Algorithms
{
    public class DfsSolver
    {
        public async Task<(bool pathFound, int visitedNodes)> SolveAsync(Node[,] grid, Node start, Node finish, Action updateUI, int delayMs, bool allowDiagonals)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            int visitedNodes = 0;

            var stack = new Stack<Node>();
            stack.Push(start);

            int[] dRow = allowDiagonals
                ? new int[] { 1, 1, 0, -1, -1, -1, 0, 1 }
                : new int[] { 1, 0, -1, 0 };
            int[] dCol = allowDiagonals
                ? new int[] { 0, 1, 1, 1, 0, -1, -1, -1 }
                : new int[] { 0, 1, 0, -1 };

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (current == finish) return (true, visitedNodes);

                if (current.Status == NodeStatus.Visited) continue;

                if (current != start)
                {
                    current.Status = NodeStatus.Visited;
                    visitedNodes++;
                    updateUI.Invoke();
                    await Task.Delay(delayMs);
                }

                for (int i = 0; i < dRow.Length; i++)
                {
                    int newRow = current.Row + dRow[i];
                    int newCol = current.Col + dCol[i];

                    if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
                    {
                        var neighbor = grid[newRow, newCol];
                        if (neighbor.Status == NodeStatus.Empty || neighbor.Status == NodeStatus.Finish)
                        {
                            if (neighbor.Status != NodeStatus.Visited)
                            {
                                neighbor.Parent = current;
                                stack.Push(neighbor);
                            }
                        }
                    }
                }
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