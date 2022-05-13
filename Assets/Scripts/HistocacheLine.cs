using UnityEngine;

public class HistocacheLine : MonoBehaviour
{
    Color c1 = Color.yellow;
    Color c2 = Color.red;

    void Start()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
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
