using PathfindingVisualizer.Client.Models;

namespace PathfindingVisualizer.Client.Algorithms
{
    public class MazeGenerator
    {
        public async Task GenerateMazeAsync(Node[,] grid, Node startNode, Node finishNode, Action updateUI, int delayMs)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (grid[r, c].Status != NodeStatus.Start && grid[r, c].Status != NodeStatus.Finish)
                    {
                        grid[r, c].Status = NodeStatus.Wall;
                    }
                }
            }
            updateUI.Invoke();

            var stack = new Stack<Node>();

            Node initial = grid[1, 1];
            if (initial == startNode || initial == finishNode) initial = grid[1, 2];

            if (initial.Status != NodeStatus.Start && initial.Status != NodeStatus.Finish)
                initial.Status = NodeStatus.Empty;

            stack.Push(initial);
            var random = new Random();

            while (stack.Count > 0)
            {
                var current = stack.Peek();
                var neighbors = GetUnvisitedNeighbors(current, grid, rows, cols, startNode, finishNode);

                if (neighbors.Count > 0)
                {
                    var next = neighbors[random.Next(neighbors.Count)];

                    int wallRow = current.Row + (next.Row - current.Row) / 2;
                    int wallCol = current.Col + (next.Col - current.Col) / 2;

                    if (grid[wallRow, wallCol].Status != NodeStatus.Start && grid[wallRow, wallCol].Status != NodeStatus.Finish)
                        grid[wallRow, wallCol].Status = NodeStatus.Empty;

                    if (next.Status != NodeStatus.Start && next.Status != NodeStatus.Finish)
                        next.Status = NodeStatus.Empty;

                    stack.Push(next);
                    updateUI.Invoke();
                    await Task.Delay(delayMs);
                }
                else
                {
                    stack.Pop();
                }
            }

            EnsureAccessible(startNode, grid, rows, cols);
            EnsureAccessible(finishNode, grid, rows, cols);
            updateUI.Invoke();
        }

        private List<Node> GetUnvisitedNeighbors(Node node, Node[,] grid, int rows, int cols, Node start, Node finish)
        {
            var neighbors = new List<Node>();
            int[] dRow = { -2, 0, 2, 0 };
            int[] dCol = { 0, 2, 0, -2 };

            for (int i = 0; i < 4; i++)
            {
                int newRow = node.Row + dRow[i];
                int newCol = node.Col + dCol[i];

                if (newRow > 0 && newRow < rows - 1 && newCol > 0 && newCol < cols - 1)
                {
                    if (grid[newRow, newCol].Status == NodeStatus.Wall)
                    {
                        neighbors.Add(grid[newRow, newCol]);
                    }
                }
            }
            return neighbors;
        }

        private void EnsureAccessible(Node target, Node[,] grid, int rows, int cols)
        {
            int[] dRow = { -1, 0, 1, 0 };
            int[] dCol = { 0, 1, 0, -1 };
            for (int i = 0; i < 4; i++)
            {
                int r = target.Row + dRow[i];
                int c = target.Col + dCol[i];
                if (r >= 0 && r < rows && c >= 0 && c < cols)
                {
                    if (grid[r, c].Status == NodeStatus.Wall) grid[r, c].Status = NodeStatus.Empty;
                }
            }
        }
    }
}