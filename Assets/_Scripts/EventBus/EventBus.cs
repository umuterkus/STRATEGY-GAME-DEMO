using System;
using UnityEngine;

public static class EventBus
{
    // To avoid any 

    public static event Action<BuildingBase> OnBuildingPlaced;
    public static event Action<BuildingBase> OnBuildingSelected;
    public static event Action<BuildingBase> OnBuildingDestroyed;
    public static event Action<BuildingDataSO> OnPlacementStarted;
    public static event Action OnPlacementCancelled;
    public static event Action<UnitData, Vector2> OnUnitProduced;

    public static void RaiseBuildingPlaced(BuildingBase building) => OnBuildingPlaced?.Invoke(building);
    public static void RaiseBuildingSelected(BuildingBase building) => OnBuildingSelected?.Invoke(building);
    public static void RaiseBuildingDestroyed(BuildingBase building) => OnBuildingDestroyed?.Invoke(building);
    public static void RaisePlacementStarted(BuildingDataSO data) => OnPlacementStarted?.Invoke(data);
    public static void RaisePlacementCancelled() => OnPlacementCancelled?.Invoke();
    public static void RaiseUnitProduced(UnitData data, Vector2 pos) => OnUnitProduced?.Invoke(data, pos);
}

