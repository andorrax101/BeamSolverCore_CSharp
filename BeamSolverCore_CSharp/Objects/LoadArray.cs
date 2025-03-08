namespace BeamSolverCore_CSharp.Objects;

public class LoadArray
{
    private readonly LoadInstance[] _loadInstances;

    public LoadInstance[] GetLoadInstances(){
        return _loadInstances;
    }

    public LoadArray(params LoadInstance[] inputLoadInstances){
        _loadInstances = inputLoadInstances;
    }

    public static LoadArray Of(params LoadInstance[] inputLoadInstances){
        return new LoadArray(inputLoadInstances);
    }
}