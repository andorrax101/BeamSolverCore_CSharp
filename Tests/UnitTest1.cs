using BeamSolverCore_CSharp.Objects;
using BeamSolverCore_CSharp.Solver;

namespace Tests;

public class Tests
{
    [SetUp]
    public void Setup(){}

    [Test]
    public void TestPointLoads(){
        double beamLength = 10.0; // meters
        double modulus = 32E9; // Pascals
        double momentOfInertia = 0.3*Math.Pow(0.6,3)/12; // 0.3m x 0.6m beam
        LoadInstance pointLoad = new LoadInstance(-10, 5.0);

        double midPointDisplacement = 10*Math.Pow(beamLength,3)/(48*modulus*momentOfInertia);

        LoadAssembly loads = new LoadAssembly();
        loads.AddPointForce(pointLoad);
        AnalysisModel model = new AnalysisModel(beamLength, modulus, momentOfInertia, loads);

        BeamSolver_SS solver = new BeamSolver_SS(model);

        double startShear = solver.GetShear(0.0);
        double endShear = solver.GetShear(10.0);
        double midMoment = solver.GetMoment(5.0);
        double midDispl = solver.GetDeflection(5.0);
        
        Assert.That(startShear, Is.EqualTo(-5.0).Within(1E-10));
        Assert.That(endShear, Is.EqualTo(5.0).Within(1E-10));
        Assert.That(midMoment, Is.EqualTo(-25.0).Within(1E-10));
        Assert.That(midDispl, Is.EqualTo(midPointDisplacement).Within(1E-10));
    }
    
    [Test]
    public void TestPointMoments(){
        double beamLength = 10.0; // meters
        double modulus = 32E9; // Pascals
        double momentOfInertia = 0.3*Math.Pow(0.6,3)/12; // 0.3m x 0.6m beam
        LoadInstance pointLoad = new LoadInstance(-10, 5.0);

        LoadAssembly loads = new LoadAssembly();
        loads.AddPointMoment(pointLoad);
        AnalysisModel model = new AnalysisModel(beamLength, modulus, momentOfInertia, loads);

        BeamSolver_SS solver = new BeamSolver_SS(model);

        double moment1 = solver.GetMoment(4.999);
        double moment2 = solver.GetMoment(5.001);
        
        Assert.That(moment1, Is.EqualTo(-4.999).Within(1E-10));
        Assert.That(moment2, Is.EqualTo(4.999).Within(1E-10));
    }

    [Test]
    public void TestUniformlyDistributedLoads(){
        double beamLength = 10.0; // meters
        double modulus = 32E9; // Pascals
        double momentOfInertia = 0.3*Math.Pow(0.6,3)/12; // 0.3m x 0.6m beam
        double midPointDisplacement = 5*10*Math.Pow(beamLength,4)/(384*modulus*momentOfInertia);

        LoadInstance start = new LoadInstance(-10, 0.0);
        LoadInstance end = new LoadInstance(-10, 10.0);

        LoadAssembly loads = new LoadAssembly();
        loads.AddDistributedForce(start, end);
        AnalysisModel model = new AnalysisModel(beamLength, modulus, momentOfInertia, loads);
        var solver = new BeamSolver_SS(model);

        double startShear = solver.GetShear(0.0);
        double endShear = solver.GetShear(10.0);
        double midMoment = solver.GetMoment(5.0);
        double midDispl = solver.GetDeflection(5.0);
        
        Assert.That(startShear, Is.EqualTo(-50.0).Within(1E-10));
        Assert.That(endShear, Is.EqualTo(50.0).Within(1E-10));
        Assert.That(midMoment, Is.EqualTo(-10*100.0/8.0).Within(1E-10));
        Assert.That(midDispl, Is.EqualTo(midPointDisplacement).Within(1E-10));
    }

    [Test]
    public void TestTriangularLoads(){
        double beamLength = 10.0; // meters
        double modulus = 32E9; // Pascals
        double momentOfInertia = 0.3*Math.Pow(0.6,3)/12; // 0.3m x 0.6m beam
        LoadInstance start = new LoadInstance(0.0, 0.0);
        LoadInstance mid = new LoadInstance(-10, 5.0);
        LoadInstance end = new LoadInstance(0.0, 10.0);

        LoadAssembly loads = new LoadAssembly();
        loads.AddDistributedForce(start, mid);
        loads.AddDistributedForce(mid, end);
        AnalysisModel model = new AnalysisModel(beamLength, modulus, momentOfInertia, loads);
        var solver = new BeamSolver_SS(model);

        double startShear = solver.GetShear(0.0);
        double endShear = solver.GetShear(10.0);
        double midMoment = solver.GetMoment(5.0);
        
        Assert.That(startShear, Is.EqualTo(-25.0).Within(1E-10));
        Assert.That(endShear, Is.EqualTo(25.0).Within(1E-10));
        Assert.That(midMoment, Is.EqualTo(-10*100.0/12.0).Within(1E-10));
    }
    [Test]
    public void TestMultipleUDLs(){
        // generate beam
        double beamLength = 5.5; // meters
        double modulus = 32E9; // Pascals
        double momentOfInertia = 0.3*Math.Pow(0.6,3)/12; // 0.3m x 0.6m beam

        LoadInstance start = new LoadInstance(19.62768, 1.5);
        LoadInstance end = new LoadInstance(19.62768, 4.0);

        LoadInstance startLandingStart = new LoadInstance(9.525,0.0);
        LoadInstance startLandingEnd = new LoadInstance(9.525,1.5);
        LoadInstance endLandingStart = new LoadInstance(9.525,4.0);
        LoadInstance endLandingEnd = new LoadInstance(9.525,5.5);

        LoadAssembly loads = new LoadAssembly();
        loads.AddDistributedForce(startLandingStart, startLandingEnd, start, end, endLandingStart, endLandingEnd);

        AnalysisModel model = new AnalysisModel(beamLength, modulus, momentOfInertia, loads);
        var solver = new BeamSolver_SS(model);

        double startShear = solver.GetShear(0.0);
        double endShear = solver.GetShear(beamLength);
        double midMoment = solver.GetMoment(beamLength/2);
        
        Assert.That(startShear, Is.EqualTo(38.8221).Within(1E-10));
        Assert.That(endShear, Is.EqualTo(-38.8221).Within(1E-10));
        Assert.That(midMoment, Is.EqualTo(62.85165).Within(1E-10));
    }
}