using System.Collections;
using UnityEngine;


public class Soldier : UnitBase, IAttacker
{
    private IDamageable attackTargetDamageable;
    private IGridEntity attackTargetGridEntity;
    private Coroutine attackCoroutine;
    public bool IsAttacking => attackCoroutine != null;

    private CombatUnitData CombatData => UnitData as CombatUnitData;

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
        if (target == null || ReferenceEquals(target, this))
            return;

        if (CombatData == null)
        {
            return;
        }

        if (!(target is IGridEntity gridEntity))
        {
            return;
        }

        CancelAttack();

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
            if (!GridManager.Instance.TryGetEntityBounds(attackTargetGridEntity, out Vector2Int targetOrigin, out Vector2Int targetSize))
            {
                CancelAttack();
                yield break;
            }

            Vector2Int ownCell = GridManager.Instance.GetGridCoordinate(transform.position);

            if (GridManager.Instance.AreWithinRange(ownCell, targetOrigin, targetSize, CombatData.AttackRange))
            {
                Debug.Log($"{name}, hedefe {CombatData.AttackDamage} hasar vurdu! Hedef Kalan Can: {attackTargetDamageable.CurrentHealth - CombatData.AttackDamage}");

                attackTargetDamageable.TakeDamage(CombatData.AttackDamage);
                yield return new WaitForSeconds(CombatData.AttackCooldown);
                continue;
            }

            Vector2Int? destinationCell = GridManager.Instance.GetNearestClearCellInRange(
                targetOrigin, targetSize, CombatData.AttackRange, ownCell);

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