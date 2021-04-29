using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HistocachingII
{
    public class WorldBuilder : MonoBehaviour
    {
        public LocationService locationService;

        public GameObject m_gpsUIText;

        public GameObject markerTemplate;

        private float[,] markerPositions = new float[,] { {-6.880282f, 107.5919408f} };

        private List<Material> materials = new List<Material>();

        private List<GameObject> markers = new List<GameObject>();

        private float latitude;
        private float longitude;

        private float heading = 0;

        private float distance = 0.0001f; // equivalent to 11.1 m

        void Awake()
        {
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
                GameObject marker = Instantiate(markerTemplate);

                marker.GetComponent<Marker>().cylinder.GetComponent<Renderer>().material = materials[index];

                markers.Add(marker);
            }

            return markers[index];
        }

        void OnLocationChanged(float altitude, float latitude, float longitude, double timestamp)
        {
            m_gpsUIText.GetComponent<TMP_Text>().text = "TANIAKP OnLocationChanged " + latitude + " - " + longitude;

            this.latitude = latitude;
            this.longitude = longitude;

            for (int i = 0; i < markerPositions.GetLength(0); ++i)
            {
                float dLat = (markerPositions[i, 0] - latitude) / distance;
                float dLon = (markerPositions[i, 1] - longitude) / distance;
                
                Vector3 position = new Vector3(dLon, 0, dLat);

                GameObject marker = GetMarker(i);
                marker.transform.position = position;
                marker.SetActive(true);

                // marker.GetComponent<Marker>().distanceLabel.text = (dLon * 11.1f) + " - " + (dLat * 11.1f);
                marker.GetComponent<Marker>().distanceLabel.text = (dLon) + " - " + (dLat);

                Debug.Log("TANIA OnLocationChanged " + marker.transform.position);
            }

            OnCompassChanged(270);
        }

        void OnCompassChanged(float heading)
        {
            m_gpsUIText.GetComponent<TMP_Text>().text = "TANIAKP OnCompassChanged " + heading;

            this.heading = heading;

            transform.transform.Rotate(0, -heading, 0, Space.World);
            // transform.Rotate(Vector3.zero, Vector3.up, heading);
        }
    }
}
