using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HistocachingII
{
    public class User : MonoBehaviour
    {
        public LocationService locationService;

        public const float UPDATE_TIME = 0.5f;

        public GameObject m_gpsUIText;

        // public GameObject m_compass;

        private Camera m_MainCamera;
        private Vector3 m_targetCompassPosition;
        private Quaternion m_targetCompassRotation;

        // Start is called before the first frame update
        void Start()
        {
            m_MainCamera = Camera.main;

            locationService.headingChangedEvent.AddListener(OnHeadingChanged);
        }

        void Update()
        {
            transform.position = Vector3.Lerp(transform.position, m_targetCompassPosition, Time.deltaTime * 5.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, m_targetCompassRotation, Time.deltaTime * 2.0f);
        }

        void Destroy()
        {
            locationService.headingChangedEvent.RemoveListener(OnHeadingChanged);
        }

        void OnHeadingChanged(float trueHeading)
        {
            m_targetCompassPosition = new Vector3(
                m_MainCamera.transform.position.x,
                0,
                m_MainCamera.transform.position.z
            );

            if (m_MainCamera.transform.localEulerAngles.x > 180f || m_MainCamera.transform.localEulerAngles.x < 20f)
                return;

            // m_gpsUIText.GetComponent<TMP_Text>().text = "true heading: " + Input.compass.trueHeading + "\n" +
            //     "camera localEulerAngles.y: " + m_MainCamera.transform.localEulerAngles.y + "\n" +
            //     "camera localEulerAngles.x: " + m_MainCamera.transform.localEulerAngles.x;

            // Orient Compass GameObject to point northward
            m_targetCompassRotation = Quaternion.Euler(0, -Input.compass.trueHeading + m_MainCamera.transform.localEulerAngles.y, 0);
        }
    }
}
