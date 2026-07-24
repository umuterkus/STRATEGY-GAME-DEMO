using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductionButtonUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button button;

    public void Setup(UnitData data, Action onClickAction)
    {
        if (iconImage != null)
            iconImage.sprite = data.UnitSprite;

        if (nameText != null)
            nameText.text = data.UnitName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickAction?.Invoke());
    }
}