using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecyclingScrollView : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private RectTransform cardPrefab;
    [SerializeField] private float cardHeight = 150f;
    [SerializeField] private float spacing = 20f;

    public event Action<RectTransform, int> OnCardSetup;

    private readonly List<RectTransform> activeCards = new List<RectTransform>();
    private ComponentPool<RectTransform> cardPool;
    private float itemHeightTotal;
    private int dataStartIndex;
    private int totalItemCount;

    public void Initialize(int itemCount)
    {
        totalItemCount = itemCount;
        itemHeightTotal = cardHeight + spacing;
        StartCoroutine(InitAfterLayout());

    }

    private IEnumerator InitAfterLayout()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();

        float viewportHeight = scrollRect.GetComponent<RectTransform>().rect.height;
        int visibleCount = Mathf.CeilToInt(viewportHeight / itemHeightTotal) + 2;

        cardPool = new ComponentPool<RectTransform>(cardPrefab, contentPanel, visibleCount);

        for (int i = 0; i < visibleCount; i++)
        {
            RectTransform card = cardPool.Get();
            card.anchoredPosition = new Vector2(0, -i * itemHeightTotal);
            activeCards.Add(card);
            OnCardSetup?.Invoke(card, i % totalItemCount);
        }

        contentPanel.sizeDelta = new Vector2(contentPanel.sizeDelta.x, totalItemCount * itemHeightTotal);
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    private void OnScroll(Vector2 normalizedPos)
    {
        if (activeCards.Count == 0) return;

        float contentPosY = contentPanel.anchoredPosition.y;

        RectTransform firstCard = activeCards[0];
        RectTransform lastCard = activeCards[activeCards.Count - 1];

        if (contentPosY > -firstCard.anchoredPosition.y + itemHeightTotal)
        {
            dataStartIndex = (dataStartIndex + 1) % totalItemCount;
            firstCard.anchoredPosition = new Vector2(0, lastCard.anchoredPosition.y - itemHeightTotal);

            int newIndex = (dataStartIndex + activeCards.Count - 1) % totalItemCount;
            OnCardSetup?.Invoke(firstCard, newIndex);

            activeCards.Add(firstCard);
            activeCards.RemoveAt(0);
        }
        else if (contentPosY < -firstCard.anchoredPosition.y)
        {
            dataStartIndex = (dataStartIndex - 1 + totalItemCount) % totalItemCount;
            lastCard.anchoredPosition = new Vector2(0, firstCard.anchoredPosition.y + itemHeightTotal);

            OnCardSetup?.Invoke(lastCard, dataStartIndex);

            activeCards.Insert(0, lastCard);
            activeCards.RemoveAt(activeCards.Count - 1);
        }
    }
}