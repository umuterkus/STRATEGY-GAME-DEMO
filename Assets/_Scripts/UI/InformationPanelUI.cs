using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InformationPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Image displayImage;
    [SerializeField] private TextMeshProUGUI displayNameText;
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
        EventBus.OnSelectableSelected += HandleSelectableSelected;
    }

    private void OnDisable()
    {
        EventBus.OnSelectableSelected -= HandleSelectableSelected;
    }

    private void HandleSelectableSelected(ISelectable selectable)
    {
        ClearProductionButtons();

        if (selectable is not IDisplayable displayable)
        {
            if (panelRoot != null)
                panelRoot.SetActive(false);
            return;
        }

        panelRoot.SetActive(true);
        displayImage.sprite = displayable.DisplayIcon;
        displayNameText.text = displayable.DisplayName;

        if (selectable is IUnitProducable producer)
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