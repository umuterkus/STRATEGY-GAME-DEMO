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
    [SerializeField] private float moveSpeed = 3f;

    // Runtime protection
    public string UnitName => unitName;
    public Sprite UnitIcon => unitIcon;
    public GameObject UnitPrefab => unitPrefab;
    public int UnitMaxHealth => unitMaxHealth;
    public float MoveSpeed => moveSpeed;

}