using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SurfaceDetector : MonoBehaviour
{
    private ARPlaneManager planeManager;
    public bool arMode;

    // Start is called before the first frame update
    void Start()
    {
        planeManager = GetComponent<ARPlaneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlanesChanged(ARPlanesChangedEventArgs planeEvent)
    {
        if (planeEvent.added.Count > 0 || planeEvent.updated.Count > 0)
        {
            planeManager.planesChanged -= PlanesChanged;
        }
    }
}
