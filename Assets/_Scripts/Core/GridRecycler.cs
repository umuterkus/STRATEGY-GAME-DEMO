using UnityEngine;


public class GridRecycler
{
    public int Columns { get; }
    public int TotalItemCount { get; }
    public int TotalRowCount { get; }
    public int TopRowIndex { get; private set; }

    public GridRecycler(int totalItemCount, int columns)
    {
        TotalItemCount = totalItemCount;
        Columns = columns;
        TotalRowCount = Mathf.CeilToInt((float)totalItemCount / columns);
        TopRowIndex = 0;
    }

    public int GetDataIndex(int rowOffset, int column)
    {
        int row = TopRowIndex + rowOffset;
        if (row < 0 || row >= TotalRowCount) return -1;

        int raw = row * Columns + column;
        return raw < TotalItemCount ? raw : -1;
    }

    public int[] AdvanceOneRow(int visibleRowCount)
    {
        int maxTopRow = Mathf.Max(0, TotalRowCount - visibleRowCount);
        if (TopRowIndex >= maxTopRow) return null;

        TopRowIndex++;

        int[] indices = new int[Columns];
        for (int col = 0; col < Columns; col++)
            indices[col] = GetDataIndex(visibleRowCount - 1, col);
        return indices;
    }

    public int[] RetreatOneRow()
    {
        if (TopRowIndex <= 0) return null;

        TopRowIndex--;

        int[] indices = new int[Columns];
        for (int col = 0; col < Columns; col++)
            indices[col] = GetDataIndex(0, col);
        return indices;
    }
}