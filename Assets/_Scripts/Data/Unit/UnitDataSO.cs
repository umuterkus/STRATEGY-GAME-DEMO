using UnityEngine;

public class UnitData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string unitName;
    [SerializeField] private Sprite unitIcon;

    [Header("Prefab")]
    [SerializeField] private GameObject unitPrefab;

    [Header("Stats")]
    [SerializeField] private int unitMaxHealth = 10;
  
    // Runtime protection
    public string UnitName => unitName;
    public Sprite UnitSprite => unitIcon;
    public GameObject UnitPrefab => unitPrefab;
    public int UnitMaxHealth => unitMaxHealth;
  
}