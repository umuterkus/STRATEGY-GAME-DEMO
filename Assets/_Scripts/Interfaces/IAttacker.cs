public interface IAttacker
{
    int AttackDamage { get; }
    void AttackTarget(IDamageable target);
    void CancelAttack();
    bool IsAttacking { get; }
}
