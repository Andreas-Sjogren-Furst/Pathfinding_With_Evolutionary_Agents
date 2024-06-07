// Written by: Andreas Sjögren Fürst (s201189)
public class FractionArithmetic 
{
    public static int GCD(int a, int b) {
        if (b == 0) 
            return a; 
        return GCD(b, a % b); 
    }

}
