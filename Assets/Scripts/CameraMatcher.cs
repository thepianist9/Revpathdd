using UnityEngine;

public class CameraMatcher : MonoBehaviour
{
    public Camera camera;
 
    void Update()
    {
        transform.rotation = camera.transform.rotation;
    }
}
