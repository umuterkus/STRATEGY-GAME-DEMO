using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveableUnit : UnitBase, IMoveable
{
    [SerializeField] private int maxPathRetries = 3;

    private bool isMoving;
    public bool IsMoving => isMoving;

    private List<Vector2Int> currentPath;
    private int currentPathIndex;
    private Vector2Int finalTargetCell;
    private int currentRetryCount;
    private Coroutine moveCoroutine;

    protected override void Die()
    {
        CancelMovement();
        base.Die();
    }

    public override void ResetUnit()
    {
        CancelMovement();
        base.ResetUnit();
    }

    public void MoveTo(Vector2 targetPosition)
    {
        Vector2Int ownCell = GridManager.Instance.GetGridCoordinate(transform.position);
        Vector2Int targetCell = GridManager.Instance.GetGridCoordinate(targetPosition);

        List<Vector2Int> path = PathfindingManager.Instance.RequestPath(ownCell, targetCell, ownCell);

        if (path == null)
            return; 

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

    protected void CancelMovement()
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