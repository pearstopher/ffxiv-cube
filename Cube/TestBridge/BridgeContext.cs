using System;
using System.Collections.Generic;

namespace Cube.TestBridge;

/// <summary>
/// What an in-game check can see: the plugin's parts and its build stamp. Also collects the numbers a
/// check measured, which go into the results file next to its pass/fail.
/// </summary>
/// <param name="window">The cube window.</param>
/// <param name="dllPath">Full path of the loaded plugin DLL.</param>
/// <param name="builtAt">The DLL's write time when the plugin loaded: the build stamp.</param>
internal sealed class BridgeContext(CubeWindow window, string dllPath, DateTime builtAt)
{
    /// <summary>The cube window, so checks can open it and read its draw cost.</summary>
    public CubeWindow Window { get; } = window;

    /// <summary>Full path of the loaded plugin DLL.</summary>
    public string DllPath { get; } = dllPath;

    /// <summary>The build stamp: when the loaded DLL was written, read at load.</summary>
    public DateTime BuiltAt { get; } = builtAt;

    /// <summary>Metrics recorded by the check that is running now; cleared before each check.</summary>
    public Dictionary<string, double> Metrics { get; } = [];

    /// <summary>Records a measured number for the current check, e.g. <c>averageMs</c>.</summary>
    /// <param name="name">The metric's name in the results file.</param>
    /// <param name="value">The measured value.</param>
    public void Metric(string name, double value) => this.Metrics[name] = value;
}
