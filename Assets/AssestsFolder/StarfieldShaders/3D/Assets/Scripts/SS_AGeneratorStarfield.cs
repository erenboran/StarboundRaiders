using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SS_AGeneratorStarfield : SS_AGenerator
{
    [Tooltip("Colors are applied to mesh.")]
    public Gradient gradient = new Gradient();

    /// <summary>
    /// Sets seed equal to _seed;
    /// </summary>
    public void SetNoise(int _seed) { seed = _seed; SetNoise(); }
    public abstract void SetNoise();

    /// <summary>
    /// Sets seed equal to _seed;
    /// </summary>
    public void SetColors(int _seed) { SetColors(_seed, gradient); }
    /// <summary>
    /// Sets gradient equal to _gradient;
    /// </summary>
    public void SetColors(Gradient _gradient) { SetColors(seed, _gradient); }
    /// <summary>
    /// Sets seed equal to _seed and gradient equal to _gradient;
    /// </summary>
    public void SetColors(int _seed, Gradient _gradient)
	{
		seed = _seed;
		gradient = _gradient;
		SetColors();
	}
    public abstract void SetColors();
}
