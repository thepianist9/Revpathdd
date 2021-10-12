using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ComputerVisionManager : MonoBehaviour
{
    public ARCameraManager cameraManager;
    public ARPlaneManager planeManager;
    public ARPointCloudManager pointCloudManager;

    public TextMeshProUGUI debugText;
    private string cameraInfoDebugText, planeDebugText, pointCloudDebugText;
    private int planeChangedCounter = 0, pointCloudChangedCounter = 0;

    void Start()
    {
        ARSession.stateChanged += OnStateChanged;
        planeManager.planesChanged += OnPlanesChanged;
        pointCloudManager.pointCloudsChanged += OnPointCloudsChanged;
    }

    void Update()
    {
        debugText.text = cameraInfoDebugText + planeDebugText + pointCloudDebugText;
    }

    void Destroy()
    {
        ARSession.stateChanged -= OnStateChanged;
        planeManager.planesChanged -= OnPlanesChanged;
        pointCloudManager.pointCloudsChanged -= OnPointCloudsChanged;
    }

    private void OnStateChanged(ARSessionStateChangedEventArgs args)
    {
        if (args.state == ARSessionState.SessionTracking)
        {
            // Use ARCameraManager to obtain the camera configurations.
            using (NativeArray<XRCameraConfiguration> configurations = cameraManager.GetConfigurations(Allocator.Temp))
            {
                if (!configurations.IsCreated || (configurations.Length <= 0))
                {
                    cameraInfoDebugText = "Cannot get XRCameraConfiguration\n";
                }
                else
                {
                    cameraInfoDebugText = "";
                    // Iterate through the list of returned configs to locate the config you want.
                    var desiredConfig = configurations[0];
                    for (int i = 1; i < configurations.Length; ++i)
                    {
                        cameraInfoDebugText += "XRCameraConfiguration[" + i + "]\n"
                            + "    framerate = " + configurations[i].framerate + "\n"
                            + "    height = " + configurations[i].height + "\n"
                            + "    width = " + configurations[i].width + "\n"
                            + "    resolution = " + configurations[i].resolution.x + " x " + configurations[i].resolution.y + "\n";
                    }
                }
            }

            cameraInfoDebugText += "Current XRCameraConfiguration\n"
                + "    framerate = " + cameraManager.currentConfiguration?.framerate + "\n"
                + "    height = " + cameraManager.currentConfiguration?.height + "\n"
                + "    width = " + cameraManager.currentConfiguration?.width + "\n"
                + "    resolution = " + cameraManager.currentConfiguration?.resolution.x + " x " + cameraManager.currentConfiguration?.resolution.y + "\n";
            
            XRCameraIntrinsics cameraIntrinsics;
            if (cameraManager.TryGetIntrinsics(out cameraIntrinsics))
            {
                cameraInfoDebugText += "XRCameraIntrinsics\n"
                + "    focalLength = " + cameraIntrinsics.focalLength.x + ", " + cameraIntrinsics.focalLength.y + "\n"
                + "    principalPoint = " + cameraIntrinsics.principalPoint.x + ", " + cameraIntrinsics.principalPoint.y + "\n"
                + "    resolution = " + cameraIntrinsics.resolution.x + ", " + cameraIntrinsics.resolution.y + "\n";
            }
            else
            {
                cameraInfoDebugText += "Cannot get XRCameraIntrinsics\n";
            }
        }
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        string text = "\n--------------------------------------------------------\n"
            + "Plane Changed Event | Counter: " + ++planeChangedCounter + "\n";

        if (args.added.Count > 0) text += "--------------------------------------------------------\n"
            + "Boundaries of added planes: ";

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

        if (args.updated.Count > 0) text += "--------------------------------------------------------\n"
            + "Boundaries of updated planes: ";

        // Iterate the list of ARPlanes updated since the last event
        foreach (ARPlane plane in args.updated)
        {
            text += "\n";
            
            foreach (Vector2 boundary in plane.boundary)
            {
                text += boundary + " ";
            }
        }

        planeDebugText = text;
    }

    private void OnPointCloudsChanged(ARPointCloudChangedEventArgs args)
    {
        string text = "\n--------------------------------------------------------\n"
            + "Point Cloud Changed Event | Counter: " + ++pointCloudChangedCounter + "\n";

        if (args.added.Count > 0) text += "--------------------------------------------------------\n"
            + "Positions of added point clouds: ";

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

        if (args.updated.Count > 0) text += "--------------------------------------------------------\n"
            + "Positions of updated point clouds: ";

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

        pointCloudDebugText = text;
    }
}
