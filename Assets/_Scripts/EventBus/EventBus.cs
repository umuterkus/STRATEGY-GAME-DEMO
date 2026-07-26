using System;
using UnityEngine;

public static class EventBus
{
    // For protection

    public static event Action<BuildingBase> OnBuildingPlaced;
    public static event Action<BuildingBase> OnBuildingDestroyed;
    public static event Action<BuildingDataSO> OnPlacementStarted;
    public static event Action<UnitData, Vector2> OnUnitProduced;
    public static event Action<ISelectable> OnSelectableSelected;

    public static void RaiseBuildingPlaced(BuildingBase building) => OnBuildingPlaced?.Invoke(building);
    public static void RaiseBuildingDestroyed(BuildingBase building) => OnBuildingDestroyed?.Invoke(building);
    public static void RaisePlacementStarted(BuildingDataSO data) => OnPlacementStarted?.Invoke(data);
    public static void RaiseUnitProduced(UnitData data, Vector2 pos) => OnUnitProduced?.Invoke(data, pos);
    public static void RaiseSelectableSelected(ISelectable selectable) => OnSelectableSelected?.Invoke(selectable); 


}

