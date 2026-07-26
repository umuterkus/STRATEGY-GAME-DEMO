using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour, IDamageable, ISelectable, IMoveable, IGridEntity, IDisplayable
{
    public UnitData UnitData { get; protected set; }
    
    protected int currentHealth;
    public int CurrentHealth => currentHealth;

    [SerializeField] private int maxPathRetries = 3;

    private bool isMoving;
    public bool IsMoving => isMoving;

    public string DisplayName => UnitData.name;
    public Sprite DisplayIcon => UnitData.UnitIcon;

    public int MaxHealth => UnitData.UnitMaxHealth;

    private List<Vector2Int> currentPath;
    private int currentPathIndex;
    private Vector2Int finalTargetCell;
    private int currentRetryCount;
    private Coroutine moveCoroutine;


    protected bool isDead;
    public event Action<UnitBase> OnDied;

    public event Action<IGridEntity> OnDespawned;

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

        CancelMovement();

        OnDied?.Invoke(this);
        OnDespawned?.Invoke(this);
    }

    public virtual void ResetUnit()
    {
        isDead = false;
        CancelMovement();
    }

    public void Select()
    {
        EventBus.RaiseSelectableSelected(this); 
    }

    public void Deselect()
    {
        EventBus.RaiseSelectableSelected(null); 
    }

    public void MoveTo(Vector2 targetPosition)
    {
        Vector2Int ownCell = GridManager.Instance.GetGridCoordinate(transform.position);
        Vector2Int targetCell = GridManager.Instance.GetGridCoordinate(targetPosition);

        List<Vector2Int> path = PathfindingManager.Instance.RequestPath(ownCell, targetCell, ownCell);

        if (path == null)
            return; // Hedefe hiçbir þekilde ulaþýlamýyor, emri yok say.

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        finalTargetCell = targetCell;
        currentPath = path;
        currentPathIndex = 0;
        currentRetryCount = 0;

        if (currentPath.Count == 0)
        {
            isMoving = false;
            return; 
        }

        isMoving = true;
        moveCoroutine = StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath()
    {
        const float minWaitSeconds = 0.4f;
        const float maxWaitSeconds = 0.6f;
        const float arrivalThreshold = 0.01f;

        while (currentPathIndex < currentPath.Count)
        {
            Vector2Int nextCell = currentPath[currentPathIndex];

            if (!GridManager.Instance.IsGridClear(nextCell))
            {
                currentRetryCount++;

                if (currentRetryCount > maxPathRetries)
                {
                    CancelMovement();
                    yield break;
                }

                yield return new WaitForSeconds(UnityEngine.Random.Range(minWaitSeconds, maxWaitSeconds));

                Vector2Int currentCell = GridManager.Instance.GetGridCoordinate(transform.position);
                List<Vector2Int> newPath = PathfindingManager.Instance.RequestPath(currentCell, finalTargetCell, currentCell);

                if (newPath != null)
                {
                    currentPath = newPath;
                    currentPathIndex = 0;
                }

                continue;
            }

            currentRetryCount = 0;
            GridManager.Instance.MoveEntity(this, nextCell);

            Vector2 targetCenter = GridManager.Instance.GetGridCenterPosition(nextCell);
            Vector3 targetWorldPos = new Vector3(targetCenter.x, targetCenter.y, transform.position.z);

            while (Vector3.Distance(transform.position, targetWorldPos) > arrivalThreshold)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, UnitData.MoveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetWorldPos;
            currentPathIndex++;
        }

        isMoving = false;
        moveCoroutine = null;
    }

    private void CancelMovement()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        currentPath = null;
        currentPathIndex = 0;
        currentRetryCount = 0;
        isMoving = false;
    }





}