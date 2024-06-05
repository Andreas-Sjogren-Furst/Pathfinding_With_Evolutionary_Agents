using System.Collections.Generic;
public class PositionComparer : IComparer<HPANode>
{
    public int Compare(HPANode x, HPANode y)
    {
        if (x == null && y == null)
            return 0;
        if (x == null)
            return -1;
        if (y == null)
            return 1;

        int compareX = x.Position.x.CompareTo(y.Position.x);
        if (compareX != 0)
            return compareX;

        return x.Position.y.CompareTo(y.Position.y);
    }
}