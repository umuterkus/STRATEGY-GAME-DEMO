using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    // This script mainly connects Pure C# Class A* to GridManager
    public static PathfindingManager Instance { get; private set; }

    private Pathfinder pathfinder;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        pathfinder = new Pathfinder(grid => GridManager.Instance.IsGridClear(grid));
    }

    // To use IMoveable units to have path
    public List<Vector2Int> RequestPath(Vector2Int start, Vector2Int target, Vector2Int ignoreGrid)
    {
        return pathfinder.FindPath(start, target, ignoreGrid);
    }
}