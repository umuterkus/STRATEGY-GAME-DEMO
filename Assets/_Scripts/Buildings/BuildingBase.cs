using System;
using UnityEngine;

public class BuildingBase : MonoBehaviour, IDamageable, ISelectable, IGridEntity, IDisplayable
{
    [SerializeField] protected BuildingDataSO buildingData;
    protected int currentHealth;
    protected Vector2Int originGrid;

    public Vector2Int OriginGrid => originGrid;
    public BuildingDataSO BuildingData => buildingData;


    // IDisplayable implementation, read from the data asset, not used in the game but i want to added anyway.
    public string DisplayName => buildingData.BuildingName;
    public Sprite DisplayIcon => buildingData.BuildingIcon;
    public int MaxHealth => buildingData.BuildingMaxHealth;
    public int CurrentHealth => currentHealth;


    // Fired when this building is destroyed, GridManager listens to free its cells
    public event Action<IGridEntity> OnDespawned;

    public virtual void Initialize(BuildingDataSO buildingData, Vector2Int origin)
    {
        this.buildingData = buildingData;
        originGrid = origin;
        currentHealth = buildingData.BuildingMaxHealth;

        // I added this for extra protection in case box colider size

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
            col.size = buildingData.GridSize;

        // Sync the visual with the SO data icon.
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sprite = buildingData.BuildingIcon;
    }

    // Called when player clicks this building
    public virtual void Select()
    {
        EventBus.RaiseSelectableSelected(this); 
    }

    public virtual void Deselect()
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