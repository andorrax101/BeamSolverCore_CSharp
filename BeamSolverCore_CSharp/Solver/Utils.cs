namespace BeamSolverCore_CSharp.Solver;

public class Utils
{
    private Utils(){}
    
    /// <summary>
    /// Returns the result of &lt; x-constant &gt; ^exponent.
    /// <p>Refer link for more details: <a href="https://en.wikipedia.org/wiki/Singularity_function">LINK</a></p>
    /// </summary>
    protected internal static double GetSingularityFunctionResult(double constant, int exponent, double x){
        if(exponent == -1 || exponent == 0){
            return x < constant ? 0.0 : 1.0;
        } else if (exponent < -1) {
            throw new Exception("Cannot solve for negative exponents");
        } else {
            if(x <= constant){
                return 0.0;
            } else {
                return Math.Pow(x - constant, exponent);
            }
        }
    }
}