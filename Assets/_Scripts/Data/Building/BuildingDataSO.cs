using UnityEngine;
[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Game/Building Data")]
public class BuildingDataSO : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string buildingName;
    [SerializeField] private Sprite buildingSprite;

    [Header("Dimensions")]
    [SerializeField] private int width;
    [SerializeField] private int height;

    [Header("Stats")]
    [SerializeField] private int buildingHealth;

    // Cannot decide prefab database or prefab in SO
    [Header("Prefab")]
    [SerializeField] private BuildingBase buildingPrefab;

    // Runtime protection
    public string BuildingName => buildingName;
    public Sprite BuildingSprite => buildingSprite;
    public Vector2Int GridSize => new Vector2Int(width, height);
    public int BuildingHealth => buildingHealth;
    public BuildingBase BuildingPrefab => buildingPrefab;
 
}