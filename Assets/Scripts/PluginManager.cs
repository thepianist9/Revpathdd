using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PluginManager : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public ARPointCloudManager pointCloudManager;

    // Start is called before the first frame update
    void Start()
    {
        planeManager.planesChanged += OnPlanesChanged;
        pointCloudManager.pointCloudsChanged += OnPointCloudsChanged;
    }

    void Destroy()
    {
        planeManager.planesChanged -= OnPlanesChanged;
        pointCloudManager.pointCloudsChanged -= OnPointCloudsChanged;
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        string text = "";

        if (args.added.Count > 0) text += "\nBoundaries of added planes: ";

        // Iterate the list of ARPlanes added since the last event
        foreach (ARPlane plane in args.added)
        {
            text += "\n";
                
            foreach (Vector2 boundary in plane.boundary)
            {
                text += boundary + " ";
            }
        }

        if (args.added.Count > 0) text += "\n\n";

        if (args.updated.Count > 0) text += "Boundaries of updated planes: ";

        // Iterate the list of ARPlanes updated since the last event
        foreach (ARPlane plane in args.updated)
        {
            text += "\n";
            
            foreach (Vector2 boundary in plane.boundary)
            {
                text += boundary + " ";
            }
        }
    }

    private void OnPointCloudsChanged(ARPointCloudChangedEventArgs args)
    {
        string text = "";

        if (args.added.Count > 0) text += "\nPositions of added point clouds: ";

        // Iterate the list of ARPointClouds added since the last event
        for (int i = 0; i < args.added.Count; ++i)
        {
            ARPointCloud pointCloud = args.added[i];

            text += "\n";

            foreach (Vector3 position in pointCloud.positions)
            {
                text += position + " ";
            }
        }

        if (args.added.Count > 0) text += "\n\n";

        if (args.updated.Count > 0) text += "Positions of updated point clouds: ";

        // Iterate the list of ARPointClouds updated since the last event
        for (int i = 0; i < args.updated.Count; ++i)
        {
            ARPointCloud pointCloud = args.updated[i];

            text += "\n";

            foreach (Vector3 position in pointCloud.positions)
            {
                text += position + " ";
            }
        }
    }
}
