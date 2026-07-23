using UnityEngine;


public static class BuildingFactory
{
    public static BuildingBase Create(BuildingDataSO buildingData, Vector2Int gridPosition)
    {
        Vector2 worldPos = GridManager.Instance.GetGridCenterPosition(gridPosition);
        BuildingBase instance = Object.Instantiate(buildingData.BuildingPrefab, worldPos, Quaternion.identity);

        instance.Initialize(buildingData, gridPosition);
        GridManager.Instance.OccupyArea(gridPosition, buildingData.GridSize);

        EventBus.OnBuildingPlaced?.Invoke(instance);
        return instance;
    }
}