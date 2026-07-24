using UnityEngine;

[CreateAssetMenu(fileName = "NewSoldierData", menuName = "Game/Soldier Data")]
public class SoldierData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string unitName;
    [SerializeField] private Sprite unitSprite;

    [Header("Prefab")]
    [SerializeField] private GameObject soldierPrefab;

    [Header("Stats")]
    [SerializeField] private int soldierHealth = 10;
    [SerializeField] private int attackDamage;

    // Runtime protection
    public string UnitName => unitName;
    public Sprite UnitSprite => unitSprite;
    public GameObject SoldierPrefab => soldierPrefab;
    public int SoldierHealth => soldierHealth;
    public int AttackDamage => attackDamage;
}