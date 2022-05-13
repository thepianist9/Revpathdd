using UnityEngine;

namespace HistocachingII
{
    public class CameraMatcher : MonoBehaviour
    {
        private Camera m_MainCamera;

        void Start()
        {
            m_MainCamera = Camera.main;
        }
    
        void Update()
        {
            transform.rotation = Quaternion.Euler(0, m_MainCamera.transform.localEulerAngles.y, 0);
        }
    }
}
