using System.Collections.Generic;
using UnityEngine;

public class Barracks : BuildingBase, IUnitProducable
{
    [SerializeField] private List<UnitData> produceableUnits;
    [SerializeField] private Vector2 spawnOffset = new Vector2(1f, 0f);

    public List<UnitData> ProduceableUnits => produceableUnits;

    public void ProduceUnit(UnitData unitData)
    {
        Vector2 spawnPos = (Vector2)transform.position + spawnOffset;
        EventBus.OnUnitProduced?.Invoke(unitData, spawnPos);
    }
}