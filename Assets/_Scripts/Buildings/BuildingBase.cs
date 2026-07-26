using System;
using UnityEngine;

public abstract class BuildingBase : MonoBehaviour, IDamageable, ISelectable, IGridEntity, IDisplayable
{
    [SerializeField] protected BuildingDataSO buildingData;
    protected int currentHealth;
    protected Vector2Int originCell;

    public Vector2Int OriginCell => originCell;
    public BuildingDataSO BuildingData => buildingData;
    
    public string DisplayName => buildingData.BuildingName;
    public Sprite DisplayIcon => buildingData.BuildingIcon;
    public int MaxHealth => buildingData.BuildingMaxHealth;
    public int CurrentHealth => currentHealth;

    public event Action<IGridEntity> OnDespawned;

    public virtual void Initialize(BuildingDataSO buildingData, Vector2Int origin)
    {
        this.buildingData = buildingData;
        originCell = origin;
        currentHealth = buildingData.BuildingMaxHealth;
    }

    public void Select()
    {
        EventBus.RaiseSelectableSelected(this); 
    }

    public void Deselect()
    {
        EventBus.RaiseSelectableSelected(null); 
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