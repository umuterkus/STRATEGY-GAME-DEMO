using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
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

        pathfinder = new Pathfinder(cell => GridManager.Instance.IsGridClear(cell));
    }

    public List<Vector2Int> RequestPath(Vector2Int start, Vector2Int target, Vector2Int ignoreCell)
    {
        return pathfinder.FindPath(start, target, ignoreCell);
    }
}