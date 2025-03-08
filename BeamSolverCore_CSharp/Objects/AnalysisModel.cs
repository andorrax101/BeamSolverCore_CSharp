namespace BeamSolverCore_CSharp.Objects;

public record AnalysisModel(
    double Length,
    double ModulusOfElasticity,
    double MomentOfInertia,
    LoadAssembly LoadAssembly);
