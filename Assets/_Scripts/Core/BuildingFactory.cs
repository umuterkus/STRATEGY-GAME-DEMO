using UnityEngine;

public class BuildingFactory : IBuildingFactory
{
    // Creates a new building instance from data, at a given world position and grid position

    public BuildingBase Create(BuildingDataSO buildingData, Vector2 worldPos, Vector2Int gridPosition)
    {
        // Spawn the prefab defined in the data asset

        BuildingBase instance = Object.Instantiate(buildingData.BuildingPrefab, worldPos, Quaternion.identity);

        // Sends Building base class
        instance.Initialize(buildingData, gridPosition);
        return instance;
    }
}