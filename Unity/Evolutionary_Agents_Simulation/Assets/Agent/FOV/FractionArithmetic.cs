using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractionArithmetic 
{
    public static Fraction LowestTerm(Fraction fraction){
        int d = GCD(fraction.numerator, fraction.denominator);
        return new Fraction(fraction.numerator/d, fraction.denominator/d);
    }

    private static int GCD(int a, int b) {
        if (b == 0) 
            return a; 
        return GCD(b, a % b); 
    }

    public static Fraction Multiply(Fraction a, Fraction b){
        Fraction newFraction = new(a.numerator * b.numerator, a.denominator * b.denominator);
        return LowestTerm(newFraction);
    }
    
    public static Fraction Divide(Fraction a, Fraction b){
        Fraction newFraction = new(a.numerator * b.denominator, a.denominator * b.numerator);
        return LowestTerm(newFraction);
    }

    public static Fraction Add(Fraction a, Fraction b){
        int newNumerator = a.numerator * b.denominator + b.numerator * a.denominator;
        int newDenominator = a.denominator * b.denominator;
        Fraction newFraction = new(newNumerator,newDenominator);
        return LowestTerm(newFraction);
    }
    public static Fraction Minus(Fraction a, Fraction b){
        int newNumerator = a.numerator * b.denominator - b.numerator * a.denominator;
        int newDenominator = a.denominator * b.denominator;
        Fraction newFraction = new(newNumerator,newDenominator);
        return LowestTerm(newFraction);
    }
}
