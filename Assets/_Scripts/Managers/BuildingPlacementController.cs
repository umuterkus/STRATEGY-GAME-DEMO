using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacementController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private BuildingDataSO currentBuildingData;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        if (currentBuildingData == null) return;
        HandlePlacementInput();
    }

    public void SetBuildingToPlace(BuildingDataSO buildingData)
    {
        currentBuildingData = buildingData;
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
            CancelPlacement();
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
    }
}