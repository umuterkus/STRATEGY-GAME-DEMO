using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductionMenuCard : MonoBehaviour
{
    [SerializeField] private Image buildingIcon;
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private Button cardButton;

    private BuildingDataSO currentData;

    private void Awake()
    {
        cardButton.onClick.AddListener(OnCardClicked);
    }

    public void Setup(BuildingDataSO buildingData)
    {
        currentData = buildingData;
        buildingIcon.sprite = buildingData.BuildingSprite;
        buildingNameText.text = buildingData.BuildingName;
    }

    private void OnCardClicked()
    {
        if (currentData == null) return;
        EventBus.RaisePlacementStarted(currentData);
    }

    private void OnDestroy()
    {
        cardButton.onClick.RemoveListener(OnCardClicked);
    }
}