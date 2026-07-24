using UnityEngine;

[CreateAssetMenu(fileName = "NewCombatUnitData", menuName = "Game/Units/Combat Unit Data")]

public class CombatUnitData : UnitData
{
    [Header("Combat")]
    [SerializeField] private int attackDamage;

    public int AttackDamage => attackDamage;
}
