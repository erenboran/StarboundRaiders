using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public abstract class SS_AGenerator2D : SS_AGenerator
{
    protected Camera m_cam;
    protected Camera Cam
    {
        get
        {
            if (m_cam == null)
                m_cam = FindObjectOfType<Camera>();
            return m_cam;
        }
    }

    [Tooltip("Automatically scale this GameObject based on the orthographic camera size.")]
    public bool autoScale = true;
	
    [SerializeField]
    [Tooltip("Enable the parallax effect - works with transform.position.")]
    protected bool parallaxEffect = true;
    [SerializeField]
    [Range(0f, 1f)]
    protected float parallaxSpeed = 0.1f;
	
    public void ParallaxEffect(bool _enable) { ParallaxEffect(_enable, parallaxSpeed); }
    public void ParallaxEffect(bool _enable, float _speed)
    {
        if (_enable)
            animate = false;
        parallaxEffect = _enable;
        parallaxSpeed = _speed;
    }

    [SerializeField]
    protected bool animate = false;
    [SerializeField]
    protected Vector2 animationSpeed = new Vector2(1, 0);

    public void Animate(bool _enable) { Animate(_enable, animationSpeed); }
    public void Animate(bool _enable, Vector2 _speed)
    {
        if (_enable)
            parallaxEffect = false;
        animate = _enable;
        animationSpeed = _speed;
    }

    [Tooltip("Match mesh size and shader bounds to orthographic camera aspect ratio.")]
    [SerializeField]
    protected bool matchAspectRatio = true;
	
    // mesh
    [Range(2, 255)]
    [SerializeField]
    protected int m_detailY = 32;
    public abstract int detailY { get; set; }
    [Range(2, 255)]
    [SerializeField]
    protected int m_detailX = 66;
    public abstract int detailX { get; set; }
    public bool generateMeshOnStart;

    protected override void OnStart()
    {
        if (autoScale)
            SetScale();

        if (generateMeshOnStart)
            GenerateMesh();

        base.OnStart();
    }

    private void Update()
    {
        if (parallaxEffect)
            ParallaxEffect();
        else if (animate)
            Animate();

        if (autoScale)
            SetScale();
    }

    /// <summary>
    /// Sets scale based on the orthographic camera size.
    /// </summary>
    public void SetScale()
    {
        transform.localScale = Vector3.one * Cam.orthographicSize * 0.2f; 
    }
	
    protected abstract void ParallaxEffect();
	
    protected abstract void Animate();

    /// <summary>
    /// Sets matchAspectRatio to _matchAspectRatio
    /// </summary>
	public void GenerateMesh(bool _matchAspectRatio)
    {
        matchAspectRatio = _matchAspectRatio;
        GenerateMesh();
    }
}
