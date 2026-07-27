using System.Collections.Generic;
using UnityEngine;


//It is the central system that tracks what occupies each grid cell,
//handles placing and moving entities, and provides range queries used by other systems attack or move to.
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private int gridWidth = 20;
    [SerializeField] private int gridHeight = 20;
    [SerializeField] private float gridSize = 1f;

    //GridMap is 2 dimensional array where each grid keeps track of who occupies
    private IGridEntity[,] gridMap;

    //where is this entity for unit tracking etc
    private readonly Dictionary<IGridEntity, (Vector2Int origin, Vector2Int size)> entityRecords = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gridMap = new IGridEntity[gridWidth, gridHeight];
    }

    //World position to grid
    public Vector2Int GetGridCoordinate(Vector2 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / gridSize);
        int y = Mathf.FloorToInt(worldPos.y / gridSize);
        return new Vector2Int(x, y);
    }

    //Grid to center of the world position
    public Vector2 GetGridCenterPosition(Vector2Int grid)
    {
        float x = grid.x * gridSize + gridSize / 2f;
        float y = grid.y * gridSize + gridSize / 2f;
        return new Vector2(x, y);
    }

    //Grid to center where it is more than a grid (1x2 2x2..)
    public Vector2 GetEntityCenterPosition(Vector2Int origin, Vector2Int size)
    {
        float x = origin.x * gridSize + (size.x * gridSize) / 2f;
        float y = origin.y * gridSize + (size.y * gridSize) / 2f;
        return new Vector2(x, y);
    }

    public bool IsGridClear(Vector2Int grid)
    {
        if (!IsInBounds(grid)) return false;
        return gridMap[grid.x, grid.y] == null;
    }

    public bool IsAreaClear(Vector2Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                if (!IsGridClear(origin + new Vector2Int(x, y)))
                    return false;
        return true;
    }

    //This is for barracks spawning units, finding nearest grid if the spawn point grid is full 
    public Vector2Int? GetNearestEmptyGrid(Vector2Int startGrid, int maxRadius = 10)
    {
        if (IsGridClear(startGrid)) return startGrid;

        for (int radius = 1; radius <= maxRadius; radius++)
        {
            List<Vector2Int> ringCells = new List<Vector2Int>();
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (Mathf.Abs(x) != radius && Mathf.Abs(y) != radius) continue;
                    ringCells.Add(startGrid + new Vector2Int(x, y));
                }
            }

            for (int i = ringCells.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (ringCells[i], ringCells[j]) = (ringCells[j], ringCells[i]);
            }

            foreach (var cell in ringCells)
            {
                if (IsGridClear(cell)) return cell;
            }
        }

        return null; 
    }

    // Registers an entity on the grid and its occupied grids.
    public bool PlaceEntity(Vector2Int origin, Vector2Int size, IGridEntity occupant)
    {
        if (!IsAreaClear(origin, size)) return false;

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                SetGrid(origin + new Vector2Int(x, y), occupant);

        entityRecords[occupant] = (origin, size);

        //When this entity despawns, its grid cells are freed automatically
        occupant.OnDespawned += UnregisterEntity;
        return true;
    }


    //Moves an entity to a new location clears its old grid, places it in the new grid if that area is free
    public bool MoveEntity(IGridEntity entity, Vector2Int newOrigin)
    {
        if (!entityRecords.TryGetValue(entity, out var record))
            return false;

        Vector2Int size = record.size;

        if (!IsAreaClear(newOrigin, size))
            return false;

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                SetGrid(record.origin + new Vector2Int(x, y), null);

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                SetGrid(newOrigin + new Vector2Int(x, y), entity);

        entityRecords[entity] = (newOrigin, size);
        return true;
    }

    //Returns an entity grid position and size if it is full,
    public bool TryGetEntityBounds(IGridEntity entity, out Vector2Int origin, out Vector2Int size)
    {
        if (entityRecords.TryGetValue(entity, out var record))
        {
            origin = record.origin;
            size = record.size;
            return true;
        }

        origin = default;
        size = default;
        return false;
    }

    // Checks the range of the targets grid
    public bool AreWithinRange(Vector2Int cell, Vector2Int origin, Vector2Int size, int range)
    {
        int dx = Mathf.Max(0, Mathf.Max(origin.x - cell.x, cell.x - (origin.x + size.x - 1)));
        int dy = Mathf.Max(0, Mathf.Max(origin.y - cell.y, cell.y - (origin.y + size.y - 1)));
        return dx <= range && dy <= range;
    }

    // Finds the closest free grid with in the range.
    public Vector2Int? GetNearestClearGridInRange(Vector2Int origin, Vector2Int size, int range, Vector2Int fromCell)
    {
        Vector2Int? best = null;
        float bestDistSqr = float.MaxValue;

        int minX = origin.x - range;
        int maxX = origin.x + size.x - 1 + range;
        int minY = origin.y - range;
        int maxY = origin.y + size.y - 1 + range;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2Int candidate = new Vector2Int(x, y);

                bool insideFootprint = candidate.x >= origin.x && candidate.x < origin.x + size.x &&
                                        candidate.y >= origin.y && candidate.y < origin.y + size.y;
                if (insideFootprint)
                    continue;

                if (!AreWithinRange(candidate, origin, size, range))
                    continue;

                if (!IsGridClear(candidate))
                    continue;

                float distSqr = (candidate - fromCell).sqrMagnitude;
                if (distSqr < bestDistSqr)
                {
                    bestDistSqr = distSqr;
                    best = candidate;
                }
            }
        }

        return best;
    }

    //Called when an entity despawns and unsubscribes from its event, clears the grid cells it occupied, and removes its record
    private void UnregisterEntity(IGridEntity occupant)
    {
        occupant.OnDespawned -= UnregisterEntity;

        if (entityRecords.TryGetValue(occupant, out var record))
        {
            for (int x = 0; x < record.size.x; x++)
                for (int y = 0; y < record.size.y; y++)
                    SetGrid(record.origin + new Vector2Int(x, y), null);

            entityRecords.Remove(occupant);
        }
    }

    //Assigns an entity to a specific grid cell
    private void SetGrid(Vector2Int grid, IGridEntity occupant)
    {
        if (!IsInBounds(grid)) return;
        gridMap[grid.x, grid.y] = occupant;
    }


    //Checks whether a given grid falls within the grid's width and height limits.
    private bool IsInBounds(Vector2Int grid)
        => grid.x >= 0 && grid.x < gridWidth && grid.y >= 0 && grid.y < gridHeight;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = new Vector3(x * gridSize, 0, 0);
            Vector3 end = new Vector3(x * gridSize, gridHeight * gridSize, 0);
            Gizmos.DrawLine(start, end);
        }
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = new Vector3(0, y * gridSize, 0);
            Vector3 end = new Vector3(gridWidth * gridSize, y * gridSize, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}