using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.InputSystem.Interactions;

public class Astar
{
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns>Path from startPos to endPos</returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Node> openList = new List<Node>()
            { new Node(startPos, null, 0, CalculateDistance(startPos, endPos)) };

        List<Node> closedList = new List<Node>();

        while (openList.Any())
        {
            Node current = openList.OrderBy(node => node.FScore).First();
            if (current.position == endPos)
                return ReconstructPath(current);

            closedList.Add(current);
            openList.Remove(current);

            List<Cell> neighborCells = grid[current.position.x, current.position.y].GetNeighbours(grid);

            foreach (Cell neighborCell in neighborCells)
            {
                // Check if already visited neighbor cell
                if (closedList.Exists(node => node.position == neighborCell.gridPosition)) continue;

                Node neighborNode = openList.Find(node => node.position == neighborCell.gridPosition);

                // If Node does not exist yet, create default Node
                if (neighborNode == null)
                    neighborNode = new Node(neighborCell.gridPosition, null, Mathf.Infinity, Mathf.Infinity);

                // Check if there is a wall between current and neighbor
                if (neighborCell.HasWall(GetWallSide(neighborCell.gridPosition, current.position))) continue;

                float tentativeGScore = current.GScore + CalculateDistance(current.position, neighborCell.gridPosition);
                if (tentativeGScore < neighborNode.GScore)
                {
                    // Found a better path to neighbor, set new parent and scores
                    neighborNode.parent = current;
                    neighborNode.GScore = tentativeGScore;
                    neighborNode.HScore = CalculateDistance(neighborCell.gridPosition, endPos);
                    
                    if (!openList.Contains(neighborNode))
                        openList.Add(neighborNode);
                }
            }
        }

        // If no path found, go to the node with the lowest H Cost
        return ReconstructPath(closedList.OrderBy(node => node.HScore).First());
    }

    private static float CalculateDistance(Vector2Int startPos, Vector2Int endPos) => Vector2.Distance(startPos, endPos);

    private static Wall GetWallSide(Vector2Int centerPos, Vector2Int wallPos)
    {
        Vector2Int distance = wallPos - centerPos;

        Wall wallSide;
        if (distance.x != 0)
            if (distance.x < 0) wallSide = Wall.LEFT;
            else wallSide = Wall.RIGHT;
        else
            if (distance.y < 0) wallSide = Wall.DOWN;
            else wallSide = Wall.UP;

        return wallSide;
    }

    private static List<Vector2Int> ReconstructPath(Node current)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        while (current != null)
        {
            path.Insert(0, current.position);

            if (current.parent == null) break;

            Node parent = current.parent;
            // In case of infinite loop
            current.parent = null;
            current = parent;
        }

        return path;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, float GScore, float HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
