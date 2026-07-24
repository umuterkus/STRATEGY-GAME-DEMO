using System.Collections.Generic;
using UnityEngine;

public class UnitFactory
{
    private readonly Transform parent;
    private readonly int initialPoolSize;
    private readonly Dictionary<UnitData, ComponentPool<UnitBase>> pools = new Dictionary<UnitData, ComponentPool<UnitBase>>();

    public UnitFactory(Transform parent, int initialPoolSize)
    {
        this.parent = parent;
        this.initialPoolSize = initialPoolSize;
    }

    public UnitBase CreateUnit(UnitData data, Vector2 spawnPosition)
    {
        if (data == null || data.UnitPrefab == null) return null;

        if (!pools.ContainsKey(data))
        {
            UnitBase prefabComponent = data.UnitPrefab.GetComponent<UnitBase>();
            if (prefabComponent == null) return null;
            pools[data] = new ComponentPool<UnitBase>(prefabComponent, parent, initialPoolSize);
        }

        UnitBase unit = pools[data].Get(); 
        unit.Initialize(data, spawnPosition);
        return unit;
    }

    public void ReturnUnit(UnitBase unit)
    {
        UnitData data = unit.UnitData;
        unit.ResetUnit();
        if (data != null && pools.ContainsKey(data))
            pools[data].Release(unit);
    }
}