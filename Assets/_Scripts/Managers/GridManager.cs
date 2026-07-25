using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private int gridWidth = 20;
    [SerializeField] private int gridHeight = 20;
    [SerializeField] private float gridSize = 1f;

    private IGridEntity[,] gridMap;

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

    public Vector2Int GetGridCoordinate(Vector2 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / gridSize);
        int y = Mathf.FloorToInt(worldPos.y / gridSize);
        return new Vector2Int(x, y);
    }

    public Vector2 GetGridCenterPosition(Vector2Int cell)
    {
        float x = cell.x * gridSize + gridSize / 2f;
        float y = cell.y * gridSize + gridSize / 2f;
        return new Vector2(x, y);
    }

    public Vector2 GetEntityCenterPosition(Vector2Int origin, Vector2Int size)
    {
        float x = origin.x * gridSize + (size.x * gridSize) / 2f;
        float y = origin.y * gridSize + (size.y * gridSize) / 2f;
        return new Vector2(x, y);
    }

    public bool IsGridClear(Vector2Int cell)
    {
        if (!IsInBounds(cell)) return false;
        return gridMap[cell.x, cell.y] == null;
    }

    public bool IsAreaClear(Vector2Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                if (!IsGridClear(origin + new Vector2Int(x, y)))
                    return false;
        return true;
    }

    public Vector2Int GetNearestEmptyGrid(Vector2Int startCell, int maxRadius = 10)
    {
        if (IsGridClear(startCell)) return startCell;

        for (int radius = 1; radius <= maxRadius; radius++)
        {
            List<Vector2Int> ringCells = new List<Vector2Int>();
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (Mathf.Abs(x) != radius && Mathf.Abs(y) != radius) continue;
                    ringCells.Add(startCell + new Vector2Int(x, y));
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

        return startCell;
    }

    public bool PlaceEntity(Vector2Int origin, Vector2Int size, IGridEntity occupant)
    {
        if (!IsAreaClear(origin, size)) return false;

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                SetGrid(origin + new Vector2Int(x, y), occupant);

        entityRecords[occupant] = (origin, size);
        occupant.OnDespawned += UnregisterEntity;
        return true;
    }

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

    private void SetGrid(Vector2Int cell, IGridEntity occupant)
    {
        if (!IsInBounds(cell)) return;
        gridMap[cell.x, cell.y] = occupant;
    }

    private bool IsInBounds(Vector2Int cell)
        => cell.x >= 0 && cell.x < gridWidth && cell.y >= 0 && cell.y < gridHeight;

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