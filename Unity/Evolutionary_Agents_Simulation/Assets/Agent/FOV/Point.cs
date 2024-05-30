using System;

public class Point
{
    public readonly int x;
    public readonly int y;
    public Point(int x, int y){
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Point other = (Point)obj;
        return x == other.x && y == other.y;
    }

    public override int GetHashCode()
    {
        // Use a hash code combination method
        return HashCode.Combine(x, y);
    }
    
}
