using System.Collections.Generic;
using UnityEngine;

public class Barracks : BuildingBase, IUnitProducable
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<UnitData> produceableUnits;

    public List<UnitData> ProduceableUnits => produceableUnits;

    public void ProduceUnit(UnitData unitData)
    {
        Vector2Int spawnGrid = GridManager.Instance.GetGridCoordinate(spawnPoint.position);
        Vector2Int? emptyGrid = GridManager.Instance.GetNearestEmptyGrid(spawnGrid);

        if (emptyGrid == null)
        {
            Debug.LogWarning($"{name}: No empty cell found near spawn point, unit production skipped.");
            return;
        }

        Vector2 spawnPos = GridManager.Instance.GetGridCenterPosition(emptyGrid.Value);
        EventBus.RaiseUnitProduced(unitData, spawnPos);
    }
}
