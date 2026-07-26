using UnityEngine;

[CreateAssetMenu(fileName = "NewCombatUnitData", menuName = "Game/Units/Combat Unit Data")]

public class CombatUnitData : UnitData
{
    [Header("Combat")]
    [SerializeField] private int attackDamage;
    [SerializeField] private int attackRange = 1;
    [SerializeField] private float attackCooldown = 1f;

    public int AttackDamage => attackDamage;
    public int AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
}
