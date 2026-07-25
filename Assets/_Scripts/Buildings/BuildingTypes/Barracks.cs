using System.Collections.Generic;
using UnityEngine;

public class Barracks : BuildingBase, IUnitProducable
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<UnitData> produceableUnits;

    public List<UnitData> ProduceableUnits => produceableUnits;

    public void ProduceUnit(UnitData unitData)
    {
        Vector2Int spawnCell = GridManager.Instance.GetGridCoordinate(spawnPoint.position);
        Vector2Int emptyGrid = GridManager.Instance.GetNearestEmptyGrid(spawnCell);
        Vector2 spawnPos = GridManager.Instance.GetGridCenterPosition(emptyGrid);
        EventBus.RaiseUnitProduced(unitData, spawnPos);
    }
}
