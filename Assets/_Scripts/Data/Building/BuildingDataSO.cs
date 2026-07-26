using UnityEngine;
[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Game/Building Data")]
public class BuildingDataSO : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string buildingName;
    [SerializeField] private Sprite buildingIcon;

    [Header("Dimensions")]
    [SerializeField] private int width;
    [SerializeField] private int height;

    [Header("Stats")]
    [SerializeField] private int buildingMaxHealth;

    // Cannot decide prefab database or prefab in SO
    [Header("Prefab")]
    [SerializeField] private BuildingBase buildingPrefab;

    // Runtime protection
    public string BuildingName => buildingName;
    public Sprite BuildingIcon => buildingIcon;
    public Vector2Int GridSize => new Vector2Int(width, height);
    public int BuildingMaxHealth => buildingMaxHealth;
    public BuildingBase BuildingPrefab => buildingPrefab;
 
}