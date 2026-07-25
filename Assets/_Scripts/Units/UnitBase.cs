using System;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour, IDamageable, ISelectable, IMoveable, IGridEntity
{
    public UnitData UnitData { get; protected set; }
    protected int currentHealth;
    public int CurrentHealth => currentHealth;
    public bool IsMoving => throw new NotImplementedException();


    protected bool isDead;
    public event Action<UnitBase> OnDied;

    public event Action<IGridEntity> OnDespawned;

    public virtual void Initialize(UnitData data, Vector2 spawnPosition)
    {
        UnitData = data;
        currentHealth = data.UnitMaxHealth;
        transform.position = spawnPosition;
        isDead = false;
        Deselect();
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

    public void Select()
    {
        Debug.Log("Selected correctly");
    }

    public void Deselect()
    {
        Debug.Log("Deselected correctly");
    }

    public void MoveTo(Vector2 targetPosition)
    {
        throw new NotImplementedException();
    }
}