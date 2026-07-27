using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// UnitManager is a singleton that manages the full lifecycle of units when a building produces a unit (for now) 
/// it creates a pool the UnitFactory
/// </summary>
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

        Transform parent = unitParent;
        // create the factory with the chosen parent and pool size
        unitFactory = new UnitFactory(parent, initialPoolSize);
    }

    // Subscribe to the unit produced event
    private void OnEnable()
    {
        EventBus.OnUnitProduced += HandleUnitProduced;
    }

    private void OnDisable()
    {
        EventBus.OnUnitProduced -= HandleUnitProduced;
    }

    //Takes a newly created unit from the factory, adds it to the active units list, places it on the grid, and subscribes to its death event.
    private void HandleUnitProduced(UnitData data, Vector2 spawnPos)
    {
        UnitBase unit = unitFactory.CreateUnit(data, spawnPos);
        if (unit == null) return;

        activeUnits.Add(unit);
        unit.OnDied += HandleUnitDied;

        Vector2Int cell = GridManager.Instance.GetGridCoordinate(spawnPos);
        GridManager.Instance.PlaceEntity(cell, Vector2Int.one, unit);
    }

    //Removes the dead unit from the active list, unsubscribes from its event, and returns it to the pool
    private void HandleUnitDied(UnitBase unit)
    {
        unit.OnDied -= HandleUnitDied;
        activeUnits.Remove(unit);

        unitFactory.ReturnUnit(unit);
    }
}