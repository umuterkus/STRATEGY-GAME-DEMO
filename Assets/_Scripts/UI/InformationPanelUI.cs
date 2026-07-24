using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InformationPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Image buildingImage;
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private Transform productionButtonContainer;
    [SerializeField] private GameObject productionButtonPrefab;

    private List<ProductionButtonUI> buttonPool = new List<ProductionButtonUI>();
    private void Awake()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }
    private void OnEnable()
    {
        EventBus.OnBuildingSelected += HandleBuildingSelected;
    }

    private void OnDisable()
    {
        EventBus.OnBuildingSelected -= HandleBuildingSelected;
    }

    private void HandleBuildingSelected(BuildingBase building)
    {
        ClearProductionButtons();
        if (building.BuildingData == null)
        {
            panelRoot.SetActive(false);
            return;
        }
        panelRoot.SetActive(true);
        buildingImage.sprite = building.BuildingData.BuildingSprite;
        buildingNameText.text = building.BuildingData.BuildingName;

        if (building is IUnitProducable producer)
        {
            int index = 0;
            foreach (UnitData soldierData in producer.ProduceableUnits)
            {
                ProductionButtonUI btnUI = GetOrCreateButton(index);
                index++;

                UnitData capturedData = soldierData;
                btnUI.Setup(capturedData, () => producer.ProduceUnit(capturedData));
                btnUI.gameObject.SetActive(true);
            }
        }
    }

    private ProductionButtonUI GetOrCreateButton(int index)
    {
        if (index < buttonPool.Count)
        {
            return buttonPool[index];
        }

        GameObject btnObj = Instantiate(productionButtonPrefab, productionButtonContainer);
        ProductionButtonUI btnUI = btnObj.GetComponent<ProductionButtonUI>();
        buttonPool.Add(btnUI);
        return btnUI;
    }

    private void ClearProductionButtons()
    {
        foreach (var btn in buttonPool)
        {
            btn.gameObject.SetActive(false);
        }
    }
}