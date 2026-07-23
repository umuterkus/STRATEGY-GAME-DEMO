using System;

public static class EventBus
{
    public static Action<BuildingBase> OnBuildingPlaced;
    public static Action<BuildingBase> OnBuildingSelected;
    public static Action<BuildingBase> OnBuildingDestroyed;
}