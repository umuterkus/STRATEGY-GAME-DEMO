using System;
using UnityEngine;

public abstract class BuildingBase : MonoBehaviour, IDamageable
{
    [SerializeField] protected BuildingDataSO buildingDataSO;   
    protected int currentHealth;                    

    public event Action<BuildingBase> OnDied;           

    public virtual void Initialize(BuildingDataSO buildingData)
    {
        buildingDataSO = buildingData;
        currentHealth = buildingData.BuildingHealth;      
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        if (currentHealth == 0) Die();
    }

    protected virtual void Die()
    {
        OnDied?.Invoke(this);   
        Destroy(gameObject);
    }
}