using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingDatabase", menuName = "Game/Building Database")]
public class BuildingDatabaseSO : ScriptableObject
{
    [SerializeField] private List<BuildingDataSO> allBuildings;
    public List<BuildingDataSO> AllBuildings => allBuildings;
}