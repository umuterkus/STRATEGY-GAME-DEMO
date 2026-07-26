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
    [SerializeField] private int columns = 2;
    [SerializeField] private float cardHeight = 150f;
    [SerializeField] private float verticalSpacing = 20f;
    [SerializeField] private float horizontalSpacing = 20f;

    private const int BufferRowCount = 1;
    public event Action<RectTransform, int> OnCardSetup;

    private readonly List<RectTransform> activeCards = new List<RectTransform>();
    private ComponentPool<RectTransform> cardPool;
    private GridRecycler recycler;

    private float rowHeightTotal;
    private float columnWidthTotal;
    private int visibleRowCount;

    public void Initialize(int itemCount)
    {
        StopAllCoroutines();

        rowHeightTotal = cardHeight + verticalSpacing;
        columnWidthTotal = cardPrefab.rect.width + horizontalSpacing;
        recycler = new GridRecycler(itemCount, columns);

        StartCoroutine(InitAfterLayout());
    }

    private void OnDestroy()
    {
        if (scrollRect != null)
            scrollRect.onValueChanged.RemoveListener(OnScroll);
    }

    private IEnumerator InitAfterLayout()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();

        float viewportHeight = scrollRect.GetComponent<RectTransform>().rect.height;
        int neededRows = Mathf.CeilToInt(viewportHeight / rowHeightTotal) + BufferRowCount;

        visibleRowCount = Mathf.Min(neededRows, recycler.TotalRowCount);

        if (visibleRowCount <= 0)
            yield break;

        int cardCount = visibleRowCount * columns;
        cardPool = new ComponentPool<RectTransform>(cardPrefab, contentPanel, cardCount);

        activeCards.Clear();
        for (int i = 0; i < cardCount; i++)
        {
            int row = i / columns;
            int col = i % columns;

            RectTransform card = cardPool.Get();
            PositionCard(card, row, col);
            activeCards.Add(card);

            SetupCard(card, recycler.GetDataIndex(row, col));
        }

        contentPanel.sizeDelta = new Vector2(contentPanel.sizeDelta.x, recycler.TotalRowCount * rowHeightTotal);

        scrollRect.onValueChanged.RemoveListener(OnScroll);
        if (visibleRowCount < recycler.TotalRowCount)
            scrollRect.onValueChanged.AddListener(OnScroll);
    }

    private void PositionCard(RectTransform card, int row, int col)
    {
        card.anchoredPosition = new Vector2(col * columnWidthTotal, -row * rowHeightTotal);
    }

    private void SetupCard(RectTransform card, int dataIndex)
    {
        bool hasData = dataIndex >= 0;
        card.gameObject.SetActive(hasData);
        if (hasData)
            OnCardSetup?.Invoke(card, dataIndex);
    }

    private void OnScroll(Vector2 normalizedPos)
    {
        if (activeCards.Count == 0) return;

        float contentPosY = contentPanel.anchoredPosition.y;
        RectTransform topLeftCard = activeCards[0];

        bool scrolledPastTopRow = contentPosY > -topLeftCard.anchoredPosition.y + rowHeightTotal;
        bool scrolledBeforeTopRow = contentPosY < -topLeftCard.anchoredPosition.y;

        if (scrolledPastTopRow)
            MoveTopRowToBottom();
        else if (scrolledBeforeTopRow)
            MoveBottomRowToTop();
    }

    private void MoveTopRowToBottom()
    {
        int[] newIndices = recycler.AdvanceOneRow(visibleRowCount);
        if (newIndices == null) return; 

        RectTransform bottomMostCard = activeCards[activeCards.Count - 1];
        float newRowY = bottomMostCard.anchoredPosition.y - rowHeightTotal;

        for (int col = 0; col < columns; col++)
        {
            RectTransform card = activeCards[col];
            card.anchoredPosition = new Vector2(col * columnWidthTotal, newRowY);
            SetupCard(card, newIndices[col]);
        }

        for (int col = 0; col < columns; col++)
        {
            RectTransform card = activeCards[0];
            activeCards.RemoveAt(0);
            activeCards.Add(card);
        }
    }

    private void MoveBottomRowToTop()
    {
        int[] newIndices = recycler.RetreatOneRow();
        if (newIndices == null) return; 

        RectTransform topMostCard = activeCards[0];
        float newRowY = topMostCard.anchoredPosition.y + rowHeightTotal;

        int bottomRowStart = activeCards.Count - columns;
        for (int col = 0; col < columns; col++)
        {
            RectTransform card = activeCards[bottomRowStart + col];
            card.anchoredPosition = new Vector2(col * columnWidthTotal, newRowY);
            SetupCard(card, newIndices[col]);
        }

        for (int col = 0; col < columns; col++)
        {
            RectTransform card = activeCards[activeCards.Count - 1];
            activeCards.RemoveAt(activeCards.Count - 1);
            activeCards.Insert(0, card);
        }
    }
}