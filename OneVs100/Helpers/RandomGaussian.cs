using System;

namespace OneVs100.Helpers;

public class RandomGaussian : Random
{
    public double BoxMuller(float minValue, float maxValue)
    {
        double value = double.NegativeInfinity;
        while ( value is < -1 or > 1 ) //Brute-force method to get values from -1 to 1, tried to do this via scaling
                                       //and clamping but could not get a satisfying result.
        {
            double a = Sample();
            double b = Sample();
            value = Math.Sqrt(-2.0 * Math.Log(a))*Math.Cos(2.0 * Math.PI * b);
        }
        double adjValue = value * 0.5 + 0.5;
        double scaledValue = adjValue*(maxValue-minValue)+minValue;
        return scaledValue;
    }
}