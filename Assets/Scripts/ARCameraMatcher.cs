using UnityEngine;

[ExecuteAlways]
public class ARCameraMatcher : MonoBehaviour
{
    public Camera sourceCamera;
 
    private Camera _myCamera;
 
    private void OnEnable()
    {
        _myCamera = GetComponent<Camera>();
    }
 
    private void OnPreRender()
    {
        if (_myCamera == null)
        {
            return;
        }
        if (sourceCamera == null)
        {
            return;
        }
        _myCamera.projectionMatrix = sourceCamera.projectionMatrix;
 
    }
}
