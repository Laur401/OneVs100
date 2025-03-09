using System;

namespace OneVs100.Helpers;

public class RandomGaussian : Random
{
    public double BoxMuller(float minValue, float maxValue, float? extreme = null, float? spread = null)
    {
        double value = double.NegativeInfinity;
        double topValue = 1;
        double bottomValue = -1;
        double adjExtreme = 0;
        if (extreme != null && extreme > minValue && extreme < maxValue)
        {
            adjExtreme = (extreme.Value - minValue) / (maxValue - minValue);
            adjExtreme = 2 * adjExtreme - 1;
        }
        while ( value < -1 || value > 1 ) //Brute-force method to get values from -1 to 1, tried to do this via scaling
                                       //and clamping but could not get a satisfying result.
        {
            double a = Sample();
            double b = Sample();
            value = Math.Sqrt(-2.0 * Math.Log(a))*Math.Cos(2.0 * Math.PI * b);
            value += adjExtreme;
            value *= spread ?? 1;
        }
        double adjValue = value * 0.5 + 0.5; //0 to 1
        double scaledValue = adjValue*(maxValue-minValue)+minValue;
        return scaledValue;
    }
}