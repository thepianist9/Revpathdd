using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HistocachingII
{
    public class PoiBillboard : MonoBehaviour
    {
        Transform m_mainCamera;

        // TODO
        private float squaredDistance;

        void Awake()
        {
            m_mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 lookPosition = transform.position - m_mainCamera.position;
            lookPosition.y = 0;
            transform.rotation = Quaternion.LookRotation(lookPosition);
        }

        public Vector3 GetPosition()
        {
            return transform.localPosition;
        }

        public float GetSquaredDistance()
        {
            return squaredDistance;
        }

        public void SetPosition(Vector3 position)
        {
            transform.localPosition = position;

            squaredDistance = (position.x * position.z) + (position.z * position.z); 
        }
    }
}
