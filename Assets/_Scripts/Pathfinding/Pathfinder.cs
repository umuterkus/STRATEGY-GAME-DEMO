using System;
using System.Collections.Generic;
using UnityEngine;


public class Pathfinder
{
    //Calculating moving diagonal so it is 1.4
    private static readonly float DiagonalCost = Mathf.Sqrt(2f);

    private static readonly Vector2Int[] StraightDirections =
    {
        new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(0, 1), new Vector2Int(0, -1)
    };

    private static readonly Vector2Int[] DiagonalDirections =
    {
        new Vector2Int(1, 1), new Vector2Int(1, -1),
        new Vector2Int(-1, 1), new Vector2Int(-1, -1)
    };

    private readonly Func<Vector2Int, bool> isCellClear;

    private class Node
    {
        public readonly Vector2Int Position;
        public Node Parent;
        public float GCost; 
        public float HCost; 
        public float FCost => GCost + HCost;

        public Node(Vector2Int position)
        {
            Position = position;
        }
    }

    public Pathfinder(Func<Vector2Int, bool> isCellClear)
    {
        this.isCellClear = isCellClear;
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target, Vector2Int ignoreCell)
    {
        if (start == target)
            return new List<Vector2Int>();

        var openList = new List<Node>();
        var closedSet = new HashSet<Vector2Int>();
        var allNodes = new Dictionary<Vector2Int, Node>();

        Node startNode = new Node(start)
        {
            GCost = 0f,
            HCost = GetDiagonalDistance(start, target)
        };
        openList.Add(startNode);
        allNodes[start] = startNode;

        while (openList.Count > 0)
        {
            Node current = GetLowestFCostNode(openList);

            if (current.Position == target)
                return ReconstructPath(current);

            openList.Remove(current);
            closedSet.Add(current.Position);

            foreach (Vector2Int neighborPos in GetNeighbors(current.Position, ignoreCell))
            {
                if (closedSet.Contains(neighborPos))
                    continue;

                bool isDiagonal = neighborPos.x != current.Position.x && neighborPos.y != current.Position.y;
                float moveCost = isDiagonal ? DiagonalCost : 1f;
                float tentativeGCost = current.GCost + moveCost;

                if (!allNodes.TryGetValue(neighborPos, out Node neighborNode))
                {
                    neighborNode = new Node(neighborPos);
                    allNodes[neighborPos] = neighborNode;
                }

                bool isInOpenList = openList.Contains(neighborNode);

                if (!isInOpenList || tentativeGCost < neighborNode.GCost)
                {
                    neighborNode.Parent = current;
                    neighborNode.GCost = tentativeGCost;
                    neighborNode.HCost = GetDiagonalDistance(neighborPos, target);

                    if (!isInOpenList)
                        openList.Add(neighborNode);
                }
            }
        }

        return null; 
    }

    private bool IsClear(Vector2Int cell, Vector2Int ignoreCell)
    {
        if (cell == ignoreCell)
            return true;

        return isCellClear(cell);
    }

    private IEnumerable<Vector2Int> GetNeighbors(Vector2Int cell, Vector2Int ignoreCell)
    {
        foreach (Vector2Int dir in StraightDirections)
        {
            Vector2Int neighbor = cell + dir;
            if (IsClear(neighbor, ignoreCell))
                yield return neighbor;
        }

        foreach (Vector2Int dir in DiagonalDirections)
        {
            Vector2Int neighbor = cell + dir;
            if (!IsClear(neighbor, ignoreCell))
                continue;

            Vector2Int horizontalNeighbor = cell + new Vector2Int(dir.x, 0);
            Vector2Int verticalNeighbor = cell + new Vector2Int(0, dir.y);

            if (IsClear(horizontalNeighbor, ignoreCell) && IsClear(verticalNeighbor, ignoreCell))
                yield return neighbor;
        }
    }

    private Node GetLowestFCostNode(List<Node> nodes)
    {
        Node best = nodes[0];
        for (int i = 1; i < nodes.Count; i++)
        {
            if (nodes[i].FCost < best.FCost ||
                (Mathf.Approximately(nodes[i].FCost, best.FCost) && nodes[i].HCost < best.HCost))
            {
                best = nodes[i];
            }
        }
        return best;
    }

    private float GetDiagonalDistance(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return (dx + dy) + (DiagonalCost - 2f) * Mathf.Min(dx, dy);
    }

    private List<Vector2Int> ReconstructPath(Node endNode)
    {
        var path = new List<Vector2Int>();
        Node current = endNode;

        while (current.Parent != null)
        {
            path.Add(current.Position);
            current = current.Parent;
        }

        path.Reverse();
        return path;
    }
}