using BeamSolverCore_CSharp.Objects;

namespace BeamSolverCore_CSharp.Solver;

/// <summary>
/// Solver class housing logic for solving simply supported beams.
/// <p>Refer source link for details in <a href="httpsin//icjong.hosted.uark.edu/docu/09.ijee.paper.pdf">LINK</a></p>
/// </summary>
public class BeamSolver_SS {
  private readonly AnalysisModel model;
  private static readonly double TOLERANCE = 1E-10;
  public BeamSolver_SS(AnalysisModel inputModel){
    model = inputModel;
  }
  private int GetDenominator(int exponent)
  {
    return exponent switch
    {
      -2 => 1,
      -1 => 1,
      -0 => 1,
      1 => 1,
      2 => 2,
      3 => 6,
      4 => 24,
      5 => 120
    };
  }

  private double PointLoadAndMomentsProcessor(double loadVal,
                                                     double EI, double distanceFromOrigin, int exponent, double x){
    return (-loadVal/(GetDenominator(exponent)* EI))
              * Utils.GetSingularityFunctionResult(distanceFromOrigin,exponent,x);
  }

  private double DistributedLoadsProcessor(
      double loadValAtStart,double loadValAtEnd,
      double EI,
      double distanceFromOriginAtStart, double distanceFromOriginAtEnd,
      int exponent, double x){
    return (1/(GetDenominator(exponent)*EI))
        *(-Utils.GetSingularityFunctionResult(distanceFromOriginAtStart,exponent,x)*loadValAtStart
          +
        Utils.GetSingularityFunctionResult(distanceFromOriginAtEnd,exponent, x)*loadValAtEnd)
        +
        (loadValAtEnd - loadValAtStart)
            /(GetDenominator(exponent +1)*EI
            * (distanceFromOriginAtEnd - distanceFromOriginAtStart ))
        *
          (-Utils.GetSingularityFunctionResult(distanceFromOriginAtStart,exponent+1, x)
          +
              Utils.GetSingularityFunctionResult(distanceFromOriginAtEnd,exponent+1, x));
  }

  private double DistributedMomentsProcessor(
      double loadVal,
      double EI,
      double distanceFromOriginAtStart, double distanceFromOriginAtEnd,
      int exponent, double x){
    return (loadVal/(GetDenominator(exponent)*EI))
        *(Utils.GetSingularityFunctionResult(distanceFromOriginAtStart,exponent,x)
        - Utils.GetSingularityFunctionResult(distanceFromOriginAtEnd,exponent,x));
  }

  private double StartShear(){
    double length = model.Length;
    return -1* BaseModelProcessor(0, length)*model.ModulusOfElasticity*model.MomentOfInertia/length;
  }

  private double BaseModelProcessor(int minExponent, double x){
    var pointLoad = model.LoadAssembly.GetPointLoads();
    var pointMoment = model.LoadAssembly.GetPointMoments();
    var distributedLoad = model.LoadAssembly.GetDistributedLoads();
    var distributedMoment = model.LoadAssembly.GetDistributedMoments();

    double EI = model.MomentOfInertia * model.ModulusOfElasticity;

    double output = 0.0;
    foreach(var pl in pointLoad){
      output += PointLoadAndMomentsProcessor(pl.Magnitude, EI, pl.DistanceFromBeamStart, minExponent+1, x);
    }

    foreach(var pm in pointMoment){
      output += PointLoadAndMomentsProcessor(pm.Magnitude, EI, pm.DistanceFromBeamStart, minExponent, x);
    }

    foreach(var dl in distributedLoad){
      var instances = dl.GetLoadInstances();
      for(int i = 0;i< instances.Length-1; i++){
        // if distance values at i and i+1 are equal, continue to next value
        double distDiff = Math.Abs(instances[i].DistanceFromBeamStart - instances[i+1].DistanceFromBeamStart);
        if(distDiff <= TOLERANCE){
          continue;
        }
        output += DistributedLoadsProcessor(instances[i].Magnitude, instances[i+1].Magnitude,
                EI, instances[i].DistanceFromBeamStart, instances[i+1].DistanceFromBeamStart,
                minExponent+2, x);
      }
    }

    foreach(var dm in distributedMoment) {
      var instances = dm.GetLoadInstances();
      for (int i = 0; i < instances.Length - 1; i++) {
        // if distance values at i and i+1 are equal, continue to next value
        double distDiff = Math.Abs(instances[i].DistanceFromBeamStart - instances[i+1].DistanceFromBeamStart);
        if(distDiff <= TOLERANCE){
          continue;
        }
        output += DistributedMomentsProcessor(instances[i].Magnitude, EI,
                instances[i].DistanceFromBeamStart, instances[i+1].DistanceFromBeamStart, minExponent + 1, x);
      }
    }
    return output;
  }

  private double StartSlope(){
    double length = model.Length;
    return
        (- StartShear()* Math.Pow(length,3)/(6*model.MomentOfInertia*model.ModulusOfElasticity)
         -
        BaseModelProcessor(2, length))/length;
  }

  public double GetDeflection(double x){
    double EI = model.MomentOfInertia * model.ModulusOfElasticity;
    return
            StartSlope() * x
            +
            StartShear()* Math.Pow(x,3)/(GetDenominator(3)*EI)
            +
            BaseModelProcessor(2, x);
  }

  public double GetSlope(double x){
    double EI = model.MomentOfInertia * model.ModulusOfElasticity;
    return
            StartSlope()
            +
            StartShear()* Math.Pow(x,2)/(2*EI)
            +
            BaseModelProcessor(1, x);
  }

  public double GetMoment(double x){
    double EI = model.MomentOfInertia * model.ModulusOfElasticity;
    return
            StartShear() *x
            +
            EI * BaseModelProcessor(0, x);
  }

  public double GetShear(double x){
    double EI = model.MomentOfInertia * model.ModulusOfElasticity;
    return
            StartShear()
            +
            EI * BaseModelProcessor(-1, x);
  }
}