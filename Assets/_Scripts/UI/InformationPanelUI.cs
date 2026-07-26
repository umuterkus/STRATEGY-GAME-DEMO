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
    [SerializeField] private ProductionButtonUI productionButtonPrefab;
    [SerializeField] private int initialPoolSize = 3;

    private ComponentPool<ProductionButtonUI> buttonPool;
    private readonly List<ProductionButtonUI> activeButtons = new List<ProductionButtonUI>();

    private void Awake()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        //Using component pool
        buttonPool = new ComponentPool<ProductionButtonUI>(productionButtonPrefab, productionButtonContainer, initialPoolSize);
    }

    //Listening player clicks
    private void OnEnable()
    {
        EventBus.OnSelectableSelected += HandleSelected;
    }

    private void OnDisable()
    {
        EventBus.OnSelectableSelected -= HandleSelected;
    }

    private void HandleSelected(ISelectable selectable)
    {
        // Clears previously selected ISelectable

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
            foreach (UnitData soldierData in producer.ProduceableUnits)
            {
                ProductionButtonUI button = buttonPool.Get();
                activeButtons.Add(button);

                UnitData capturedData = soldierData;
                button.Setup(capturedData, () => producer.ProduceUnit(capturedData));
            }
        }
    }

    private void ClearProductionButtons()
    {
        foreach (var button in activeButtons)
            buttonPool.Release(button);

        activeButtons.Clear();
    }
}