using System;
using UnityEngine;

public abstract class BuildingBase : MonoBehaviour, IDamageable
{
    [SerializeField] protected BuildingDataSO buildingDataSO;   
    protected int currentHealth;                    
    protected Vector2Int originCell;                

    public event Action<BuildingBase> OnDied;           

    public virtual void Initialize(BuildingDataSO buildingData, Vector2Int origin)
    {
        buildingDataSO = buildingData;
        originCell = origin;
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