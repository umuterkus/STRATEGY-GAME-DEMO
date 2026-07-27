using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays a unit's icon and name on a button, when clicked 
/// Acts as the MVC layer.
/// </summary>
public class ProductionButtonUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button button;

    public void Setup(UnitData data, Action onClickAction)
    {
        if (iconImage != null)
            iconImage.sprite = data.UnitIcon;

        if (nameText != null)
            nameText.text = data.UnitName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickAction?.Invoke());
    }
}