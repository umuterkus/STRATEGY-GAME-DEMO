using System;
using UnityEngine;

//The foundation every unit shares health, selection, death, and display info. Doesnt assume a unit can move or fight.
public abstract class UnitBase : MonoBehaviour, IDamageable, ISelectable, IGridEntity, IDisplayable
{
    public UnitData UnitData { get; protected set; }

    protected int currentHealth;
    public int CurrentHealth => currentHealth;

    // IDisplayable implementation, not used but ready incase.

    public string DisplayName => UnitData.UnitName;
    public Sprite DisplayIcon => UnitData.UnitIcon;
    public int MaxHealth => UnitData.UnitMaxHealth;

    protected bool isDead;

    public event Action<UnitBase> OnDied; // This is different then despawn, UI/score updating, unitmanager pool
    public event Action<IGridEntity> OnDespawned; // Units, Building etc will use when used empty grid cell.


    // Called by UnitFactory, after the unit is taken from the pool
    public virtual void Initialize(UnitData data, Vector2 spawnPosition)
    {
        UnitData = data;
        currentHealth = data.UnitMaxHealth;
        transform.position = spawnPosition;
        isDead = false;
    }

    public virtual void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        if (currentHealth == 0) Die();
    }

    protected virtual void Die()
    {
        isDead = true;
        OnDied?.Invoke(this);
        OnDespawned?.Invoke(this);
    }

    public virtual void ResetUnit()
    {
        isDead = false;
    }

    public virtual void Select()
    {
        EventBus.RaiseSelectableSelected(this);
    }

    public virtual void Deselect()
    {
        EventBus.RaiseSelectableSelected(null);
    }
}