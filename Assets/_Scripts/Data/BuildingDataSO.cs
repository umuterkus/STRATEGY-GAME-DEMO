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


    // Runtime protection
    public string BuildingName => buildingName;
    public Sprite BuildingSprite => buildingSprite;
    public int Width => width;
    public int Height => height;
    public int BuildingHealth => buildingHealth;
 
}