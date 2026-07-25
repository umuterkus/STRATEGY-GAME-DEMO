using System;
using UnityEngine;

public abstract class BuildingBase : MonoBehaviour, IDamageable, ISelectable, IGridEntity
{
    [SerializeField] protected BuildingDataSO buildingData;
    protected int currentHealth;
    protected Vector2Int originCell;

    public Vector2Int OriginCell => originCell;
    public BuildingDataSO BuildingData => buildingData;
    public int CurrentHealth => currentHealth;

    public event Action<IGridEntity> OnDespawned;

    public virtual void Initialize(BuildingDataSO buildingData, Vector2Int origin)
    {
        this.buildingData = buildingData;
        originCell = origin;
        currentHealth = buildingData.BuildingHealth;
    }

    public void Select()
    {
        EventBus.RaiseBuildingSelected(this);
    }

    public void Deselect()
    {
        EventBus.RaiseBuildingSelected(null);
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        if (currentHealth == 0) Die();
    }

    protected virtual void Die()
    {
        OnDespawned?.Invoke(this);

        EventBus.RaiseBuildingDestroyed(this);

        Destroy(gameObject);
    }
}