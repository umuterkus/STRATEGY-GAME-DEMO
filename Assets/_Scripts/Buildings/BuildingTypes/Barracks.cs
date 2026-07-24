using System.Collections.Generic;
using UnityEngine;

public class Barracks : BuildingBase, IUnitProducer
{
    [SerializeField] private List<SoldierData> produceableUnits;
    [SerializeField] private Vector2 spawnOffset = new Vector2(1f, 0f);

    public List<SoldierData> ProduceableUnits => produceableUnits;

    public void ProduceUnit(SoldierData unitData)
    {
        Vector2 spawnPos = (Vector2)transform.position + spawnOffset;
        EventBus.OnUnitProduced?.Invoke(unitData, spawnPos);
    }
}