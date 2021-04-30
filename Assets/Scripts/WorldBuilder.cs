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

        private float[,] markerPositions = new float[,] { { -6.879016f, 107.592136f } };

        private List<Material> materials = new List<Material>();

        private List<GameObject> markers = new List<GameObject>();

        private float latitude = float.MinValue;
        private float longitude = float.MinValue;

        private float heading = float.MinValue;

        private float distance = 0.001f; // equivalent to 111.1 m

        private Vector3 cameraRotation;

        void Awake()
        {
            m_mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;

            cameraRotation = m_mainCamera.rotation.eulerAngles;

            for (int i = 0; i < 6; ++i)
            {
                materials.Add(Resources.Load("Materials/" + i) as Material);
            }
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

        }

        GameObject GetMarker(int index)
        {
            if (markers.Count < index + 1)
            {
                GameObject marker = Instantiate(markerTemplate, transform);

                marker.GetComponent<Marker>().cylinder.GetComponent<Renderer>().material = materials[index];

                markers.Add(marker);
            }

            return markers[index];
        }

        void OnLocationChanged(float altitude, float latitude, float longitude, double timestamp)
        {
            // m_gpsUIText.GetComponent<TMP_Text>().text = "TANIAKP OnLocationChanged " + latitude + " - " + longitude;

            this.latitude = latitude;
            this.longitude = longitude;

            for (int i = 0; i < markerPositions.GetLength(0); ++i)
            {
                // float dLat = (markerPositions[i, 0] - latitude) / distance;
                // float dLon = (markerPositions[i, 1] - longitude) / distance;

                SetMarker(i);
                
                // Vector3 position = new Vector3(dLon, 0, dLat);

                // GameObject marker = GetMarker(i);
                // marker.transform.position = position;
                // marker.SetActive(true);

                // marker.GetComponent<Marker>().distanceLabel.text = (dLon * 11.1f) + " - " + (dLat * 11.1f);

                // Debug.Log("TANIA OnLocationChanged " + marker.transform.position);
            }

            if (first)
            {
                first = false;
                OnCompassChanged(0);
            }
        }

        void OnCompassChanged(float heading)
        {
            // m_gpsUIText.GetComponent<TMP_Text>().text = "TANIAKP OnCompassChanged " + heading;

            this.heading = heading;

            for (int i = 0; i < markerPositions.GetLength(0); ++i)
            {
                SetMarker(i);
            }

            transform.rotation = Quaternion.Euler(0, heading, 0);

            // Debug.Log("TANIA OnLocationChanged " + transform.position);

            // transform.Rotate(0, -heading, 0, Space.Self);
            // transform.Rotate(Vector3.zero, Vector3.up, heading);

            // Vector3 rot = m_mainCamera.rotation.eulerAngles;
            // cameraRotation = rot;
            
            // m_gpsUIText.GetComponent<TMP_Text>().text = "TANIAKP OnCompassChanged " + (rot - cameraRotation);
        }

        void SetMarker(int index)
        {
            if (Mathf.Approximately(latitude, float.MinValue) || Mathf.Approximately(longitude, float.MinValue) || 
                Mathf.Approximately(heading, float.MinValue))
                return;

            float x = (markerPositions[index, 1] - longitude) / distance;
            float z = (markerPositions[index, 0] - latitude) / distance;

            GameObject marker = GetMarker(index);

            marker.transform.position = new Vector3(x, 0, z);
            marker.SetActive(true);

            marker.GetComponent<Marker>().distanceLabel.text = (x * 111.1f) + " - " + (z * 111.1f);
        }
    }
}
