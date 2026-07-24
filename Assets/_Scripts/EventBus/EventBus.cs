using System;
using UnityEngine;

public static class EventBus
{
    public static Action<BuildingBase> OnBuildingPlaced;
    public static Action<BuildingBase> OnBuildingSelected;
    public static Action<BuildingBase> OnBuildingDestroyed;

    public static Action<BuildingDataSO> OnPlacementStarted;
    public static Action OnPlacementCancelled;

    public static Action<UnitData, Vector2> OnUnitProduced;
}