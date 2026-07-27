using UnityEngine;


/// <summary>
/// Wires up building database to infinite scroll view with events.
/// </summary>
public class ProductionMenuUI : MonoBehaviour
{

    
    [SerializeField] private RecyclingScrollView scrollView;
    [SerializeField] private BuildingDatabaseSO buildingDatabase;

    private void Start()
    {
        scrollView.OnCardSetup += HandleCardSetup;
        scrollView.Initialize(buildingDatabase.AllBuildings.Count); // Tells scroll view how many building in that database.
    }

    private void OnDestroy()
    {
        scrollView.OnCardSetup -= HandleCardSetup;
    }

    private void HandleCardSetup(RectTransform cardRect, int dataIndex)
    {
        //Setups the card
        cardRect.GetComponent<ProductionMenuCard>().Setup(buildingDatabase.AllBuildings[dataIndex]);
    }
}