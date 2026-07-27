using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// In the RTS games I play, the barracks never stop spawning units if the area is blocked. 
/// To me, the correct behavior is that units should spawn at the closest possible point to spawnpoint, even if it means jumping over other soldiers. 
/// This can be changed if necessary, but since the PDF does not provide enough detail, I considered this approach.
/// </summary>


public class Barracks : BuildingBase, IUnitProducable
{
    // The spawn point is manually assigned via the Inspector
    // I Choose to do this way right now, later it can be changed maybe right clicking and selecting a grid to spawnpoint for soldiers automaticly moves that grid.
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<UnitData> produceableUnits;

    public List<UnitData> ProduceableUnits => produceableUnits;


    // Called when player clicks a production button for this barracks

    public void ProduceUnit(UnitData unitData)
    {
        // Convert spawn point world position to grid coordinate
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
