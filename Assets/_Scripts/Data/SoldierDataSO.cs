using UnityEngine;

[CreateAssetMenu(fileName = "NewSoldierData", menuName = "Game/Soldier Data")]
public class SoldierData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string unitName;
    [SerializeField] private Sprite unitSprite;

    [Header("Stats")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int attackDamage;

    // Runtime protection
    public string UnitName => unitName;
    public Sprite UnitSprite => unitSprite;
    public int MaxHealth => maxHealth;
    public int AttackDamage => attackDamage;
}