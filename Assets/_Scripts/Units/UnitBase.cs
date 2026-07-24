using System;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour, IDamageable, ISelectable
{
    public UnitData UnitData { get; protected set; } 

    protected int currentHealth;
    public int CurrentHealth => currentHealth;

    public event Action<UnitBase> OnDied;

    protected bool isDead;
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
    }

    public virtual void ResetUnit()
    {
        currentHealth = 0;
    }

    public void Select()
    {
        Debug.Log("Selected correctly");
    }

    public void Deselect()
    {
        Debug.Log("Deselected correctly");
    }
}