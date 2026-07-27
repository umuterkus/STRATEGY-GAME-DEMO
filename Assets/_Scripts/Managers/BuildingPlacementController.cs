using UnityEngine;

public class BuildingPlacementController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform buildingsContainer;
    private BuildingDataSO currentBuildingData;
    private GameObject previewInstance;
    private SpriteRenderer[] previewRenderers;
    private IBuildingFactory buildingFactory;
    public bool IsPlacing => currentBuildingData != null;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        buildingFactory = new BuildingFactory();
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

        // Initialize() is skipped for the preview, so assign the icon manually here.
        foreach (var renderer in previewRenderers)
            renderer.sprite = buildingData.BuildingIcon;
    }

    // Shared by preview and placement so both always agree on the same cell.
    private Vector2Int GetOriginCellCenteredOnMouse(Vector2 mouseWorldPos, Vector2Int size)
    {
        Vector2Int mouseCell = GridManager.Instance.GetGridCoordinate(mouseWorldPos);
        return new Vector2Int(mouseCell.x - size.x / 2, mouseCell.y - size.y / 2);
    }

    private void UpdatePreviewPositionAndColor()
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int targetCell = GetOriginCellCenteredOnMouse(mouseWorldPos, currentBuildingData.GridSize);
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
        Vector2Int targetCell = GetOriginCellCenteredOnMouse(mouseWorldPos, currentBuildingData.GridSize);

        if (!GridManager.Instance.IsAreaClear(targetCell, currentBuildingData.GridSize))
            return;

        Vector2 worldPos = GridManager.Instance.GetEntityCenterPosition(targetCell, currentBuildingData.GridSize);

        BuildingBase instance = buildingFactory.Create(currentBuildingData, worldPos, targetCell);
        instance.transform.SetParent(buildingsContainer);

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