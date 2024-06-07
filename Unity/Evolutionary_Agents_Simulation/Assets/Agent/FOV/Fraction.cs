// Written by: Andreas Sjögren Fürst (s201189)
public class Fraction
{
    public int numerator;
    public int denominator;

    public Fraction(int numerator){
        this.numerator = numerator;
        denominator = 1;
    }
    public Fraction(int numerator, int denominator){
        if(denominator == 0) throw new System.Exception("denominator can't be 0 for type Fraction");
        int gcd = FractionArithmetic.GCD(numerator,denominator);
        this.numerator = numerator/gcd;
        this.denominator = denominator/gcd;
    }

    public float EvaluateFraction(){
        return (float)numerator/denominator;
    }
}
