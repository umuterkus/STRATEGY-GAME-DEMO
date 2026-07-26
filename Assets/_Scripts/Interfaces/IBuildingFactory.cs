using UnityEngine;

public interface IBuildingFactory
{
    BuildingBase Create(BuildingDataSO buildingData, Vector2 worldPos, Vector2Int gridPosition);
}