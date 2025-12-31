using System.Numerics;

namespace DiegoG.MonoGame.Extended;

public static class WaveForms<T> where T : struct, ITrigonometricFunctions<T>
{
    public static T Sine(T amplitude, T time, T frequency, T horizontalPhaseRadians = default, T verticalPhaseRadians = default)
        => (amplitude * T.Sin(T.CreateSaturating(2) * T.Pi * frequency * time + horizontalPhaseRadians)) + verticalPhaseRadians;
    
    public static T SineWithAngularFrequency(T amplitude, T time, T angularFrequencyRadians, T horizontalPhaseRadians = default, T verticalPhaseRadians = default)
        => (amplitude * T.Sin(angularFrequencyRadians * time + horizontalPhaseRadians)) + verticalPhaseRadians;
}