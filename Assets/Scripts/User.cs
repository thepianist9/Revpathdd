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

        Vector3 m_targetCompassPosition;
        Quaternion m_targetCompassRotation;

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

        void Update()
        {
            m_compass.transform.position = Vector3.Lerp(m_compass.transform.position, m_targetCompassPosition, Time.deltaTime * 5.0f);
            m_compass.transform.rotation = Quaternion.Slerp(m_compass.transform.rotation, m_targetCompassRotation, Time.deltaTime * 2.0f);
        }

        void Destroy()
        {
            locationService.compassChangedEvent.RemoveListener(OnCompassChanged);
        }

        void OnCompassChanged(float trueHeading)
        {
            m_targetCompassPosition = new Vector3(
                m_mainCamera.position.x,
                0,
                m_mainCamera.position.z
            );

            float cameraRotation = m_mainCamera.transform.rotation.y * 180f;
            float cameraAngle = cameraRotation < 0 ? -cameraRotation : 360f - cameraRotation;

            m_gpsUIText.GetComponent<TMP_Text>().text = "true heading: " + (Input.compass.trueHeading) + "\n" +
                "camera eulerAngles.y: " + m_mainCamera.transform.eulerAngles.y + "\n" +
                "camera rotation.y: " + cameraAngle;

            // Orient compass to point northward
            m_targetCompassRotation = Quaternion.Euler(0, -Input.compass.trueHeading + cameraAngle, 0);
        }
    }
}
