namespace BeamSolverCore_CSharp.Objects;

public struct LoadInstance
{
    public LoadInstance(double magnitude, double distanceFromBeamStart)
    {
        Magnitude = magnitude; 
        DistanceFromBeamStart = distanceFromBeamStart;
    }

    public double Magnitude { get; }
    public double DistanceFromBeamStart { get; }
}