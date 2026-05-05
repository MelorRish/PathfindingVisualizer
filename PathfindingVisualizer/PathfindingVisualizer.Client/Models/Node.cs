namespace PathfindingVisualizer.Client.Models
{
    public class Node
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public NodeStatus Status { get; set; } = NodeStatus.Empty;

        public Node? Parent { get; set; }

        public Node(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
}