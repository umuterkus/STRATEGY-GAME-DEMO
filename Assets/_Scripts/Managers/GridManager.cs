using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField] private int gridWidth = 20;
    [SerializeField] private int gridHeight = 20;
    [SerializeField] private float cellSize = .5f;

    private bool[,] isOccupied;

    private BuildingBase[,] buildingGrid;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        isOccupied = new bool[gridWidth, gridHeight];

        buildingGrid = new BuildingBase[gridWidth, gridHeight];
    }

    public Vector2Int GetGridCoordinate(Vector2 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.y / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector2 GetGridCenterPosition(Vector2Int cell)
    {
        float x = cell.x * cellSize + cellSize / 2f;
        float y = cell.y * cellSize + cellSize / 2f;
        return new Vector2(x, y);
    }

    private bool IsCellClear(Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= gridWidth || cell.y < 0 || cell.y >= gridHeight)
            return false;
        return !isOccupied[cell.x, cell.y];
    }

    private void SetOccupied(Vector2Int cell, bool occupied, BuildingBase building = null)
    {
        if (cell.x < 0 || cell.x >= gridWidth || cell.y < 0 || cell.y >= gridHeight)
            return;

        isOccupied[cell.x, cell.y] = occupied;
        buildingGrid[cell.x, cell.y] = building;
    }

    public bool IsAreaClear(Vector2Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (!IsCellClear(origin + new Vector2Int(x, y)))
                    return false;
            }
        }
        return true;
    }

    public void OccupyArea(Vector2Int origin, Vector2Int size, BuildingBase building)
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                SetOccupied(origin + new Vector2Int(x, y), true, building);
    }

    public void ClearGrids(Vector2Int origin, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)  
                SetOccupied(origin + new Vector2Int(x, y), false, null);
    }


    public BuildingBase GetBuildingAt(Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= gridWidth || cell.y < 0 || cell.y >= gridHeight)
            return null;
        return buildingGrid[cell.x, cell.y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = new Vector3(x * cellSize, 0, 0);
            Vector3 end = new Vector3(x * cellSize, gridHeight * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = new Vector3(0, y * cellSize, 0);
            Vector3 end = new Vector3(gridWidth * cellSize, y * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}