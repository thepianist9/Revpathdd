using TMPro;
using UnityEngine;

namespace HistocachingII
{
    public class CameraMatcher : MonoBehaviour
    {
        private Transform m_mainCamera;

        void Awake()
        {
            m_mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
        }
    
        void Update()
        {
            transform.rotation = Quaternion.Euler(0, m_mainCamera.transform.localEulerAngles.y, 0);
        }
    }
}
