using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacementController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float previewScale = 1f;

    private BuildingDataSO currentBuildingData;
    private GameObject previewInstance;
    private SpriteRenderer previewRenderer;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        EventBus.OnPlacementStarted += StartPlacement;
        EventBus.OnPlacementCancelled += CancelPlacement;
    }

    private void OnDisable()
    {
        EventBus.OnPlacementStarted -= StartPlacement;
        EventBus.OnPlacementCancelled -= CancelPlacement;
    }

    private void Update()
    {
        if (currentBuildingData == null) return;
        UpdatePreviewPositionAndColor();
        HandlePlacementInput();
    }

    public void StartPlacement(BuildingDataSO buildingData)
    {
        CancelPlacement();
        currentBuildingData = buildingData;

        previewInstance = new GameObject("PlacementPreview");
        previewInstance.transform.localScale = new Vector3(previewScale, previewScale, 1f);

        previewRenderer = previewInstance.AddComponent<SpriteRenderer>();
        previewRenderer.sprite = buildingData.BuildingSprite;
        previewRenderer.sortingOrder = 10;
    }

    private void UpdatePreviewPositionAndColor()
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int targetCell = GridManager.Instance.GetGridCoordinate(mouseWorldPos);
        Vector2 centerPos = GridManager.Instance.GetGridCenterPosition(targetCell);

        previewInstance.transform.position = centerPos;

        bool isClear = GridManager.Instance.IsAreaClear(targetCell, currentBuildingData.GridSize);
        previewRenderer.color = isClear ? new Color(0f, 1f, 0f, 0.4f) : new Color(1f, 0f, 0f, 0.4f);
    }

    private void HandlePlacementInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int targetCell = GridManager.Instance.GetGridCoordinate(mouseWorldPos);
            TryPlaceBuilding(targetCell);
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    private void TryPlaceBuilding(Vector2Int gridCoordinate)
    {
        if (!GridManager.Instance.IsAreaClear(gridCoordinate, currentBuildingData.GridSize))
            return;

        BuildingFactory.Create(currentBuildingData, gridCoordinate);
        CancelPlacement();
    }

    private void CancelPlacement()
    {
        currentBuildingData = null;

        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
            previewRenderer = null;
        }
    }
}