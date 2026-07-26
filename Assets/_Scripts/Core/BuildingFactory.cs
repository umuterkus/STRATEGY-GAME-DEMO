using UnityEngine;

public class BuildingFactory : IBuildingFactory
{
    public BuildingBase Create(BuildingDataSO buildingData, Vector2 worldPos, Vector2Int gridPosition)
    {
        BuildingBase instance = Object.Instantiate(buildingData.BuildingPrefab, worldPos, Quaternion.identity);
        instance.Initialize(buildingData, gridPosition);
        return instance;
    }
}