using System;

public class GridRecycler
{
    // Column count in the panel
    public int Columns { get; }

    // Total building count
    public int TotalItemCount { get; }

    // Total building count devided by 2
    public int TotalRowCount { get; }
    public int TopRowIndex { get; private set; }


    // Init
    public GridRecycler(int totalItemCount, int columns)
    {
        TotalItemCount = totalItemCount;
        Columns = columns;
        TotalRowCount = (int)Math.Ceiling((double)totalItemCount / columns);
        TopRowIndex = 0;
    }

    public int GetDataIndex(int rowOffset, int column)
    {
        int row = TopRowIndex + rowOffset;
        if (row < 0 || row >= TotalRowCount) return -1;

        int raw = row * Columns + column;
        return raw < TotalItemCount ? raw : -1;
    }

    // Called when the user scrolls down. Moves the visible window one row forward

    public int[] AdvanceOneRow(int visibleRowCount)
    {
        int maxTopRow = Math.Max(0, TotalRowCount - visibleRowCount);
        if (TopRowIndex >= maxTopRow) return null;

        TopRowIndex++;

        int[] indices = new int[Columns];
        for (int col = 0; col < Columns; col++)
            indices[col] = GetDataIndex(visibleRowCount - 1, col);
        return indices;
    }

    // Called when the user scrolls up. Moves the visible window one row back
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