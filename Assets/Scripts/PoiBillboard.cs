using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HistocachingII
{
    [System.Serializable]
    public class POIClickedEvent : UnityEvent<string>
    {

    }

    public class POIBillboard : MonoBehaviour
    {
        private Camera m_MainCamera;

        // private string m_Id;

        // public string Id { get { return m_Id; } set { m_Id = value; } }

        public POIClickedEvent POIClickedEvent;

        // TODO
        private float squaredDistance;

        // Start is called before the first frame update
        void Start()
        {
            m_MainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 lookPosition = transform.position - m_MainCamera.transform.position;
            lookPosition.y = 0;
            transform.rotation = Quaternion.LookRotation(lookPosition);

            // if (Input.GetMouseButtonDown(0))
            // {  
            //     POIClickedEvent?.Invoke(id);

            //     Ray ray = m_mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            //     RaycastHit hit;
            //     if (Physics.Raycast(ray, out hit))
            //     {  
            //         POIClickedEvent?.Invoke(id);

            //         if (hit.transform.name == "Quad")
            //         {
            //             Debug.Log("OK");
            //         }
            //         else
            //         {
            //             Debug.Log("NOK");
            //         }
            //     }
            // }  
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
