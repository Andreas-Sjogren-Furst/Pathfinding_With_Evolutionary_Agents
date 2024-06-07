// written by: Gustav Clausen s214940

public class Node
{
    public int Id { get; set; }
    public double X { get; set; }
    public double Y { get; set; }

    public Node(int id, double x, double y)
    {
        Id = id;
        X = x;
        Y = y;
    }

    public override int GetHashCode()
    {
        // Create a hash code that is based on the position.
        // You might use a combination of x and y coordinates to do this.
        return Id;
    }

    public override bool Equals(object obj)
    {
        if (obj is Node other)
        {
            return this.Id == other.Id;
        }
        return false;
    }
}