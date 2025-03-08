namespace BeamSolverCore_CSharp.Objects;

public class LoadAssembly
{
    private readonly IList<LoadInstance> _pointLoads = new List<LoadInstance>();
    private readonly IList<LoadInstance> _pointMoments = new List<LoadInstance>();
    private readonly IList<LoadArray> _distributedLoads = new List<LoadArray>();
    private readonly IList<LoadArray> _distributedMoments = new List<LoadArray>();

    public IList<LoadInstance> GetPointLoads(){
        return _pointLoads;
    }
    public IList<LoadInstance> GetPointMoments(){
        return _pointMoments;
    }
    public IList<LoadArray> GetDistributedLoads(){
        return _distributedLoads;
    }
    public IList<LoadArray> GetDistributedMoments(){
        return _distributedMoments;
    }

    public void AddPointForce(LoadInstance instance){
        _pointLoads.Add(instance);
    }
    public void AddPointMoment(LoadInstance instance){
        _pointMoments.Add(instance);
    }
    public void AddDistributedForce(params LoadInstance[] instances){
        _distributedLoads.Add(LoadArray.Of(instances));
    }
    public void AddDistributedForce(List<LoadInstance> instances){
        _distributedLoads.Add(LoadArray.Of(instances.ToArray()));
    }
    public void AddDistributedMoment(params LoadInstance[] instances){
        _distributedMoments.Add(LoadArray.Of(instances));
    }
    public void AddDistributedMoment(List<LoadInstance> instances){
        _distributedMoments.Add(LoadArray.Of(instances.ToArray()));
    }
}