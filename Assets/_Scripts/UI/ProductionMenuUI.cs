using UnityEngine;

public class ProductionMenuUI : MonoBehaviour
{
    [SerializeField] private RecyclingScrollView scrollView;
    [SerializeField] private BuildingDatabaseSO buildingDatabase;

    private void Start()
    {
        scrollView.OnCardSetup += HandleCardSetup;
        scrollView.Initialize(buildingDatabase.AllBuildings.Count);
    }

    private void OnDestroy()
    {
        scrollView.OnCardSetup -= HandleCardSetup;
    }

    private void HandleCardSetup(RectTransform cardRect, int dataIndex)
    {
        cardRect.GetComponent<ProductionMenuCard>().Setup(buildingDatabase.AllBuildings[dataIndex]);
    }
}