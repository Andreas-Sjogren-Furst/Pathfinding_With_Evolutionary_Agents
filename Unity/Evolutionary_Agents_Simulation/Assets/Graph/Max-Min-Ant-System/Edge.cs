// written by: Gustav Clausen s214940

public class Edge
{
    public Node Source { get; set; }
    public Node Destination { get; set; }
    public double Distance { get; set; }

    public Edge(Node source, Node destination, double distance)
    {
        Source = source;
        Destination = destination;
        Distance = distance;
    }


    public override int GetHashCode()
    {
        return Source.Id * 1000 + Destination.Id;
    }

    public override bool Equals(object obj)
    {
        if (obj is Edge other)
        {
            return this.Source.Id == other.Source.Id && this.Destination.Id == other.Destination.Id;
        }
        return false;
    }

    //  any time => ændrer på objektiv funktionen, og bruge den tidligere løsning, som warm start løsning. 
    //  Hvor lang tid tager før systemet tiltager sig.
    //  Sammenlignet med at det starte helt. 
}