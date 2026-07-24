using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    [SerializeField] private Transform unitParent;
    [SerializeField] private int initialPoolSize = 3;

    private UnitFactory unitFactory;
    private readonly List<UnitBase> activeUnits = new List<UnitBase>();
    public List<UnitBase> ActiveUnits => activeUnits;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Transform parent = unitParent != null ? unitParent : transform;
        unitFactory = new UnitFactory(parent, initialPoolSize);
    }

    private void OnEnable()
    {
        EventBus.OnUnitProduced += HandleUnitProduced;
    }

    private void OnDisable()
    {
        EventBus.OnUnitProduced -= HandleUnitProduced;
    }

    private void HandleUnitProduced(UnitData data, Vector2 spawnPos)
    {
        UnitBase unit = unitFactory.CreateUnit(data, spawnPos);
        if (unit == null) return;

        activeUnits.Add(unit);
        unit.OnDied += HandleUnitDied;
    }

    private void HandleUnitDied(UnitBase unit)
    {
        unit.OnDied -= HandleUnitDied;
        activeUnits.Remove(unit);
        unitFactory.ReturnUnit(unit);
    }
}