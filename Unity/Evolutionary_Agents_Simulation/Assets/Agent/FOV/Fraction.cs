using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        this.numerator = numerator;
        this.denominator = denominator;
        FractionArithmetic.LowestTerm(this);
    }

    public float EvaluateFraction(){
        return numerator/denominator;
    }
}
