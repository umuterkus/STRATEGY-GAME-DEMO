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

        // Create a small pool of buttons
        buttonPool = new ComponentPool<ProductionButtonUI>(productionButtonPrefab, productionButtonContainer, initialPoolSize);
    }

    // Listen for any selection change in the game (buildings or units right now)
    private void OnEnable()
    {
        EventBus.OnSelectableSelected += HandleSelected;
    }

    private void OnDisable()
    {
        EventBus.OnSelectableSelected -= HandleSelected;
    }


    // Called every time selection changes
    private void HandleSelected(ISelectable selectable)
    {
        // Clears previously selected ISelectable

        ClearProductionButtons();


        // If the selected thing has no displayable info, just hide the panel
        if (selectable is not IDisplayable displayable)
        {
            if (panelRoot != null)
                panelRoot.SetActive(false);
            return;
        }

        panelRoot.SetActive(true);
        displayImage.sprite = displayable.DisplayIcon;
        displayNameText.text = displayable.DisplayName;

        // If this entity can also produce units, build a button per producible unit
        if (selectable is IUnitProducable producer)
        {
            foreach (UnitData unitData in producer.ProduceableUnits)
            {
                ProductionButtonUI button = buttonPool.Get();
                activeButtons.Add(button);

                UnitData capturedData = unitData;
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