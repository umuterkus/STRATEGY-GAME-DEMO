using UnityEngine;

public class BuildingPlacementController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private BuildingDataSO currentBuildingData;
    private GameObject previewInstance;
    private SpriteRenderer[] previewRenderers;
    public bool IsPlacing => currentBuildingData != null;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        EventBus.OnPlacementStarted += StartPlacement;
    }

    private void OnDisable()
    {
        EventBus.OnPlacementStarted -= StartPlacement;
    }

    private void Update()
    {
        if (!IsPlacing) return;
        UpdatePreviewPositionAndColor();
    }

    private void StartPlacement(BuildingDataSO buildingData)
    {
        CancelPlacement();
        currentBuildingData = buildingData;
        if (buildingData.BuildingPrefab == null) return;

        previewInstance = Instantiate(buildingData.BuildingPrefab.gameObject);
        BuildingBase buildingScript = previewInstance.GetComponent<BuildingBase>();
        if (buildingScript != null)
            buildingScript.enabled = false;

        Collider2D col = previewInstance.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        previewRenderers = previewInstance.GetComponentsInChildren<SpriteRenderer>();
    }

    private void UpdatePreviewPositionAndColor()
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int targetCell = GridManager.Instance.GetGridCoordinate(mouseWorldPos);
        Vector2 centerPos = GridManager.Instance.GetEntityCenterPosition(targetCell, currentBuildingData.GridSize);
        previewInstance.transform.position = centerPos;

        bool isClear = GridManager.Instance.IsAreaClear(targetCell, currentBuildingData.GridSize);
        Color tintColor = isClear ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
        foreach (var sr in previewRenderers)
            sr.color = tintColor;
    }

    public void TryPlace()
    {
        if (!IsPlacing) return;

        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int targetCell = GridManager.Instance.GetGridCoordinate(mouseWorldPos);

        if (!GridManager.Instance.IsAreaClear(targetCell, currentBuildingData.GridSize))
            return;

        Vector2 worldPos = GridManager.Instance.GetEntityCenterPosition(targetCell, currentBuildingData.GridSize);

        BuildingBase instance = BuildingFactory.Create(currentBuildingData, worldPos, targetCell);

        bool occupied = GridManager.Instance.PlaceEntity(targetCell, currentBuildingData.GridSize, instance);
        if (!occupied)
        {
            Destroy(instance.gameObject);
            return;
        }

        EventBus.RaiseBuildingPlaced(instance);

        CancelPlacement();
    }

    public void CancelPlacement()
    {
        currentBuildingData = null;
        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
            previewRenderers = null;
        }
    }
}