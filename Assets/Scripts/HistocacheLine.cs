using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistocacheLine : MonoBehaviour
{
    Color c1 = Color.yellow;
    Color c2 = Color.red;
    int lengthOfLineRenderer = 2;

    void Start()
    {
        // LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        // lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        // lineRenderer.widthMultiplier = 0.2f;
        // lineRenderer.positionCount = lengthOfLineRenderer;

        // var points = new Vector3[lengthOfLineRenderer]; 
        // points[0] = new Vector3(0.0f, 5.0f, 0.0f);
        // points[1] = new Vector3(0.0f, 5.0f, 5.0f);
        // lineRenderer.SetPositions(points);

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        // float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
    }

    public void SetPositions(Vector3[] points)
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.SetPositions(points);
    }
}
