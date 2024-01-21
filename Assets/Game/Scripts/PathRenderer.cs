using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RL.Paths;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathRenderer : MonoBehaviour
{
    public IReadOnlyPath Path;

    private LineRenderer LineRenderer;

    private void Awake()
    {
        LineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        Vector2[] points = Path.GetPointsPositions();
        LineRenderer.positionCount = points.Length;
        LineRenderer.SetPositions(points.Select((p) => (Vector3)p).ToArray());
    }

    void Update()
    {
        
    }
}
