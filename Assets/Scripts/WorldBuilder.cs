using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HistocachingII
{
    public class WorldBuilder : MonoBehaviour
    {
        Transform m_mainCamera;

        public LocationService locationService;

        public GameObject m_gpsUIText;

        public GameObject markerTemplate;

        private float[,] markerPositions;

        private List<Material> materials = new List<Material>();

        private List<GameObject> markers = new List<GameObject>();

        private GameObject billboard;

        // private Vector3 left = new Vector3(-5, 0, 20);
        // private Vector3 right = new Vector3(5, 0, 20);
        // private Vector3 bottom = new Vector3(0, 0, 5);

        private float gpsLatitude = float.MinValue;
        private float gpsLongitude = float.MinValue;

        private float compassHeading = float.MinValue;

        private float distance = 0.0001f; // equivalent to 11.1 m

        private Vector3 cameraRotation;

        private float previousYRotationAngle = 0f;

        void Awake()
        {
            m_mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;

            cameraRotation = m_mainCamera.rotation.eulerAngles;

            for (int i = 0; i < 6; ++i)
            {
                materials.Add(Resources.Load("Materials/" + i) as Material);
            }

            Data data = new Data();
            markerPositions = data.poiLocations;
        }

        // Start is called before the first frame update
        void Start()
        {
            locationService.locationChangedEvent.AddListener(OnLocationChanged);
            locationService.compassChangedEvent.AddListener(OnCompassChanged);

            // OnLocationChanged(0, Constants.PIBU_LAT, Constants.PIBU_LONG, 0);
        }

        void Destroy()
        {
            locationService.locationChangedEvent.RemoveListener(OnLocationChanged);
            locationService.compassChangedEvent.RemoveListener(OnCompassChanged);
        }

        // Update is called once per frame
        void Update()
        {
            // Quaternion q = Quaternion.Euler(0, previousYRotationAngle, 0);

            GameObject m = null;

            string text;

            for (int i = 0; i < markers.Count; ++i)
            {
                GameObject gameObject = markers[i];

                Vector3 target = gameObject.transform.position - transform.position;
                float angle = Vector3.Angle(target, GetComponent<Camera>().forward);

                if (angle >= -30 && angle <= 30) // 60° FOV
                {
                    text += i + ", " + gameObject.transform.position + "\n";
                }
            }

            m_gpsUIText.GetComponent<TMP_Text>().text = "fov: " + text + "\n";
        }

        GameObject GetMarker(int index)
        {
            if (markers.Count < index + 1)
            {
                GameObject marker = Instantiate(markerTemplate, transform, false);

                // marker.GetComponent<Marker>().cylinder.GetComponent<Renderer>().material = materials[index];

                markers.Add(marker);
            }

            return markers[index];
        }

        void SetMarker(int index)
        {
            if (Mathf.Approximately(gpsLatitude, float.MinValue) || Mathf.Approximately(gpsLongitude, float.MinValue) || 
                Mathf.Approximately(compassHeading, float.MinValue))
                return;

            Vector2 offset = Conversions.GeoToUnityPosition(markerPositions[index, 0], markerPositions[index, 1], gpsLatitude, gpsLongitude);

            GameObject marker = GetMarker(index);

            // Reposition
            marker.transform.localPosition = new Vector3(offset.y, 0, offset.x);

            // Rescale
            float scale = 1 + (Mathf.Max(offset.x, offset.y) / 50);
            marker.transform.localScale = new Vector3(scale, scale, scale);

            marker.SetActive(true);

            // marker.GetComponent<Marker>().distanceLabel.text = (offset.x) + " | " + (offset.y);
        }

        void OnLocationChanged(float altitude, float gpsLatitude, float gpsLongitude, double timestamp)
        {
            // m_gpsUIText.GetComponent<TMP_Text>().text = "TANIAKP OnLocationChanged " + latitude + " - " + longitude;

            this.gpsLatitude = gpsLatitude;
            this.gpsLongitude = gpsLongitude;

            for (int i = 0; i < markerPositions.GetLength(0); ++i)
            {
                SetMarker(i);
            }

            // OnCompassChanged(0);
        }

        void OnCompassChanged(float compassHeading)
        {
            // m_gpsUIText.GetComponent<TMP_Text>().text = "TANIAKP OnCompassChanged " + heading;

            this.compassHeading = compassHeading;

            for (int i = 0; i < markerPositions.GetLength(0); ++i)
            {
                SetMarker(i);
            }

            if (m_mainCamera.transform.localEulerAngles.x > 180f || m_mainCamera.transform.localEulerAngles.x < 20f)
                return;

            float newYRotationAngle = -compassHeading + m_mainCamera.transform.localEulerAngles.y;
            if (newYRotationAngle < 0)
                newYRotationAngle = newYRotationAngle + 360;

            // difference threshold for world rotation
            if (Mathf.Abs(previousYRotationAngle - newYRotationAngle) > 30f) {
                transform.rotation = Quaternion.Euler(0, newYRotationAngle, 0);
                // Quaternion targetRotation = Quaternion.Euler(0, newYRotationAngle, 0);
                // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);
                previousYRotationAngle = newYRotationAngle;
            }

            // m_gpsUIText.GetComponent<TMP_Text>().text = "true heading: " + compassHeading + "\n" +
            //     "camera localEulerAngles.y: " + m_mainCamera.transform.localEulerAngles.y + "\n" +
            //     "newYRotationAngle: " + newYRotationAngle;
        }
   }
}
