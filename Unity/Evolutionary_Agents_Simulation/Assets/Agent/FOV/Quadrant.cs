
public class Quadrant
{
    public readonly int north = 0;
    public readonly int east = 1;
    public readonly int south = 2;
    public readonly int west = 3;
    private readonly Point origin;
    private readonly int cardinal;
    public Quadrant(int cardinal, Point origin){
        this.cardinal = cardinal;
        this.origin = origin;
    }

    public Point Transform(Point tile){
        int row = tile.x;
        int col = tile.y;
        if(cardinal == south) return new Point(origin.x + col, origin.y - row);
        if(cardinal == north) return new Point(origin.x + col, origin.y + row);
        if(cardinal == east) return new Point(origin.x + row, origin.y + col);
        if(cardinal == west) return new Point(origin.x - row, origin.y + col);
        throw new System.Exception("Invalid cardinal value");
    }

}
