# Beam Solver Core - C#
A simple 2D beam solver library for simply supported beams capable of solving any vertical load arrangement
using the model formula approach noted in I.C.Jong's paper: [LINK](https://icjong.hosted.uark.edu/docu/09.ijee.paper.pdf)

# Feature Set
* Linear static analysis of 2D simply supported beams
* Support for load cases and combinations (WIP)
* Support for any arbitrary arrangement of vertical loads, including
  * Arbitrarily placed concentrated forces and moments
  * Uniformly distributed loads and load segments
  * Triangular loads
  * Trapezoidal loads
  * etc.

# Limitations
* Forces and moments that aren't perpendicular to the beam span are not supported
* Boundary conditions other than those for simply supported beams aren't supported
* No support for nonlinear analyses
* Non-uniformly distributed moments are currently not supported
* This solver is based on the bernoulli beam theory, so effects of shear deformations are ignored

# WIP Items
* Interactive GUI for generating models and adding loads
* Plots for diagrams

# Prerequisites
* C#13 with .NET 9 or above

# Usage and Examples
## Sign Convention
Refer `ANALYSIS_MANUAL.md`([LINK](ANALYSIS_MANUAL.md))for details on the sign convention used by this library.
## Concentrated Loads
Concentrated forces and moments may be defined using the `LoadInstance` object:
```java
// instantiates a concentrated load of magnitude 5 units acting 3 units away from the beam start
var pointForce = new LoadInstance(5.0, 3.0);
```
Note that the `LoadInstance` object itself makes no distinction between forces and moments. 
This distinction is enforced when creating the `LoadAssembly` object:
```java
var pointForce = new LoadInstance(5.0, 3.0);
var pointMoment = new LoadInstance(2.0, 3.0);

// a load assembly is a collection of different loads (usually all loads under the same load case)
var loadAssembly = new LoadAssembly();
loadAssembly.AddPointForce(pointForce); // LoadInstance object saved as a concentrated load
loadAssembly.AddPointMoment(pointMoment); // LoadInstance object saved as a concentrated moemnt
```
## Distributed Loads
Distributed forces and moments may be defined in two ways: 

* Adding in comma separated instances of `LoadInstance` objects:
```java
// define a trapezoidal distributed force where
// - force value at 3 units away from beam origin is 5.0
// - force value at 6 units away from beam origin is 2.0
var start = new LoadInstance(5.0, 3.0);
var end = new LoadInstance(2.0, 6.0);

var loadAssembly = new LoadAssembly();
loadAssembly.AddDistributedForce(start, end); // LoadInstance objects saved as a distributed force
```

* Adding in a list of `LoadInstance` objects : 
```java
List<LoadInstance> loads = getLoadInstances(); // method returns a large list of varying loads (e.g. a sinusoidal wave load)

var loadAssembly = new LoadAssembly();
loadAssembly.AddDistributedForce(loads); // LoadInstance objects saved as a distributed force
```
The above is also valid for the `addDistributedMoments()` method on the `LoadAssmembly` class.

## Model Assembly
Solving the beam requires the generation of an `AnalysisModel` object. This object takes in the following params as method args:
* beam length
* modulus of elasticity
* second moment of area about the strong (bending) axis
* a `LoadAssembly` object denoting the configuration of applied loads

Note that the solver does NOT account for unit systems. It is therefore imperative that the values used when creating the
`AnalysisModel` are consistent.

An example of model assembly (using SI units) is shown below:
```java
double beamLength = 10.0; // meters
double modulus = 32E9; // Pascals
double momentOfInertia = 0.3*Math.pow(0.6,3)/12; // 0.3m x 0.6m rectangular beam
LoadInstance pointLoad = new LoadInstance(10, 5.0); // point load of value 10 applied at mid point of beam

LoadAssembly loads = new LoadAssembly();
loads.AddPointForce(pointLoad);
AnalysisModel model = new AnalysisModel(beamLength, modulus, momentOfInertia, loads);
```

# Model Solving
To solve a model, one simply needs to instantiate a solver instance and pass in the `AnalysisModel` object.
```java
  BeamSolver_SS solver = new BeamSolver_SS(model);
```
Each `BeamSolver_SS` instance exposes 4 methods: 
* `GetShear()`
* `GetMoment()`
* `GetSlope()`
* `GetDeflection()`

Each of these methods takes in a distance value - specifically the distance away from the origin of the beam.
```java
BeamSolver_SS solver = new BeamSolver_SS(model);
double startShear = solver.GetShear(0.0);
double endShear = solver.GetShear(10.0);
double midMoment = solver.GetMoment(5.0);
double midDispl = solver.GetDeflection(5.0);
double midSlope = solver.GetSlope(5.0);
```
