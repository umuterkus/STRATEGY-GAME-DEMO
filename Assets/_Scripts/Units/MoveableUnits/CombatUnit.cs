using System.Collections;
using UnityEngine;

/// <summary>
/// Adds combat on top of MoveableUnit, attacking targets, checking range, dealing damage.
/// </summary>
public class CombatUnit : MoveableUnit, IAttacker
{
    private IDamageable attackTargetDamageable;
    private IGridEntity attackTargetGridEntity;
    private Coroutine attackCoroutine;
    public bool IsAttacking => attackCoroutine != null;

    private CombatUnitData CombatData => UnitData as CombatUnitData; //using as for null check in case 

    public int AttackDamage => CombatData.AttackDamage;

    protected override void Die()
    {
        CancelAttack();
        base.Die();
    }

    public override void ResetUnit()
    {
        CancelAttack();
        base.ResetUnit();
    }

    public void AttackTarget(IDamageable target)
    {
        if (target == null || ReferenceEquals(target, this)) // Do not let unit attack himself
            return;

        if (CombatData == null)
        {
            return;
        }

        if (!(target is IGridEntity gridEntity))
        {
            return;
        }

        // Clears any previous attacks
        CancelAttack();

        // If enemy dies it will alerted by HandleAttackTargetDespawned so can cancel the attack.
        attackTargetDamageable = target;
        attackTargetGridEntity = gridEntity;
        attackTargetGridEntity.OnDespawned += HandleAttackTargetDespawned;

        
        attackCoroutine = StartCoroutine(AttackRoutine());
    }

    public void CancelAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        if (attackTargetGridEntity != null)
            attackTargetGridEntity.OnDespawned -= HandleAttackTargetDespawned;

        attackTargetDamageable = null;
        attackTargetGridEntity = null;
    }

    private void HandleAttackTargetDespawned(IGridEntity despawned)
    {
        CancelAttack();
    }

    private IEnumerator AttackRoutine()
    {
        const float noDestinationRetryDelay = 0.5f;

        while (true)
        {
            // If somehow cannot get enemy grid, the attack gets canceled
            if (!GridManager.Instance.TryGetEntityBounds(attackTargetGridEntity, out Vector2Int targetOrigin, out Vector2Int targetSize))
            {
                CancelAttack();
                yield break;
            }

            
            Vector2Int ownGrid = GridManager.Instance.GetGridCoordinate(transform.position);


            // Check if the enemy near the range
            if (GridManager.Instance.AreWithinRange(ownGrid, targetOrigin, targetSize, CombatData.AttackRange))
            {

                attackTargetDamageable.TakeDamage(CombatData.AttackDamage);
                yield return new WaitForSeconds(CombatData.AttackCooldown);
                continue;
            }

            Vector2Int? destinationCell = GridManager.Instance.GetNearestClearGridInRange(
                targetOrigin, targetSize, CombatData.AttackRange, ownGrid);

            if (destinationCell == null)
            {
                yield return new WaitForSeconds(noDestinationRetryDelay);
                continue;
            }

            Vector2 destinationCenter = GridManager.Instance.GetGridCenterPosition(destinationCell.Value);

            MoveTo(destinationCenter);

            yield return new WaitUntil(() => !IsMoving);
        }
    }
}