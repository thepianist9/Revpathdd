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

        private float gpsLatitude = float.MinValue;
        private float gpsLongitude = float.MinValue;

        private float compassHeading = float.MinValue;

        private float distance = 0.00001f; // equivalent to 1.11 m

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

            OnLocationChanged(0, Constants.PIBU_LAT, Constants.PIBU_LONG, 0);
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

        void SetMarker(int index)
        {
            if (Mathf.Approximately(gpsLatitude, float.MinValue) || Mathf.Approximately(gpsLongitude, float.MinValue) || 
                Mathf.Approximately(compassHeading, float.MinValue))
                return;

            Vector2 offset = Conversions.GeoToUnityPosition(markerPositions[0, 0], markerPositions[0, 1], gpsLatitude, gpsLongitude);

            GameObject marker = GetMarker(index);

            marker.transform.localPosition = new Vector3(offset.y, 0, offset.x);
            marker.SetActive(true);

            // marker.GetComponent<Marker>().distanceLabel.text = (offset.x * 111.1f) + " - " + (offset.y * 111.1f);
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

            OnCompassChanged(0);
        }

        void OnCompassChanged(float compassHeading)
        {
            // m_gpsUIText.GetComponent<TMP_Text>().text = "TANIAKP OnCompassChanged " + heading;

            this.compassHeading = compassHeading;

            for (int i = 0; i < markerPositions.GetLength(0); ++i)
            {
                SetMarker(i);
            }

            transform.rotation = Quaternion.Euler(0, compassHeading, 0);
        }
   }
}
