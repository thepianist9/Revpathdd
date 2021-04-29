using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HistocachingII
{
    public class User : MonoBehaviour
    {
        public LocationService locationService;

        Transform m_mainCamera;
        // Transform m_compass;

        public const float UPDATE_TIME = 0.5f;

        public GameObject m_gpsUIText;

        public GameObject m_compass;    

        void Awake()
        {
            // Input.compass.enabled = true;
            // Input.location.Start();
            // // m_compass = transform.Find("Compass").transform;

            m_mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
        }

        // Start is called before the first frame update
        void Start()
        {
            locationService.compassChangedEvent.AddListener(OnCompassChanged);
        }

        void Destroy()
        {
            locationService.compassChangedEvent.RemoveListener(OnCompassChanged);
        }

        void OnCompassChanged(float trueHeading)
        {
            // transform.position = new Vector3(
            //     m_mainCamera.position.x,
            //     0,
            //     m_mainCamera.position.z
            // );

            // Orient compass to point northward
            m_compass.transform.rotation = Quaternion.Euler(0, -Input.compass.trueHeading, 0);
        }
    }
}
