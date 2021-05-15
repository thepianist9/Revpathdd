using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;

namespace HistocachingII
{
    public class WorldBuilder : MonoBehaviour
    {
        private NetworkManager networkManager;

        private List<POI> previousPOIList, currentPOIList;

        public Transform m_mainCamera;

        public LocationService locationService;

        public GameObject m_gpsUIText;

        public GameObject markerTemplate;
        public GameObject photoTemplate;

        private float[,] markerPositions;

        private List<GameObject> markers = new List<GameObject>();

        private GameObject billboard;
        private GameObject activeBillboard;

        // Altitude
        private float altitude = float.MinValue;

        // Location
        private float gpsLatitude = float.MinValue;
        private float gpsLongitude = float.MinValue;

        // Compass
        private float compassHeading = float.MinValue;

        // private Queue<float> compassHeadings = new Queue<float>();

        private Quaternion targetRotation;

        // Camera
        // private Vector3 cameraRotation;

        private float previousYRotationAngle = 0f;

        public ARPlaneManager arPlaneManager;

        void Awake()
        {
            // m_mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;

            // cameraRotation = m_mainCamera.rotation.eulerAngles;

            networkManager = new NetworkManager();

            previousPOIList = new List<POI>();
            currentPOIList = new List<POI>();

            Data data = new Data();
            markerPositions = data.poiLocations;

            targetRotation = transform.rotation;
        }

        // Start is called before the first frame update
        void Start()
        {
            locationService.locationChangedEvent.AddListener(OnLocationChanged);
            locationService.compassChangedEvent.AddListener(OnCompassChanged);

            arPlaneManager.planesChanged += OnPlanesChanged;

            GetPOIs();
        }

        void Destroy()
        {
            locationService.locationChangedEvent.RemoveListener(OnLocationChanged);
            locationService.compassChangedEvent.RemoveListener(OnCompassChanged);

            arPlaneManager.planesChanged -= OnPlanesChanged;
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = m_mainCamera.localPosition;
            // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);

            if (Input.deviceOrientation != DeviceOrientation.Portrait)
                return;

            // TODO: move this somewhere else and only update every 0.5 second
            int count = 0;
            GameObject m = null;

            string text = "";

            for (int i = 0; i < markers.Count; ++i)
            {
                GameObject gameObject = markers[i];

                Vector3 target = gameObject.transform.position - transform.position;
                float angle = Vector3.Angle(target, m_mainCamera.forward);

                // TODO: find real FOV calculation
                if (angle <= 15) // 60° FOV
                {
                    // m_gpsUIText.GetComponent<TMP_Text>().text = "camera: " + m_mainCamera.forward + " | target: " + target;

                    count += 1;
                    m = gameObject;

                    text += i + " " + gameObject.transform.position + " " + gameObject.transform.localPosition + "\n";
                }
            }

            // m_gpsUIText.GetComponent<TMP_Text>().text = "fov: " + text + "\n";

            if (count == 1)
            {
                // TODO
                // if (m.GetComponent<PoiBillboard>().GetSquaredDistance() <= 400) // squared distance is less than 400 m
                if (m.transform.localPosition.x * m.transform.localPosition.x + m.transform.localPosition.z * m.transform.localPosition.z <= 400)
                {
                    if (billboard == null)
                        billboard = Instantiate(photoTemplate, transform, false);

                    billboard.transform.localPosition = new Vector3(m.transform.localPosition.x, 0, m.transform.localPosition.z);
                    billboard.SetActive(true);
                    
                    activeBillboard = m;
                }
                else
                {
                    activeBillboard = null;
                }
            }
            else
            {
                if (billboard)
                    billboard.SetActive(false);

                activeBillboard = null;
            }
        }

        GameObject GetMarker(int index)
        {
            if (markers.Count < index + 1)
                markers.Add(Instantiate(markerTemplate, transform, false));

            return markers[index];
        }

        void SetMarker(int index)
        {
            Vector2 offset = Conversions.GeoToUnityPosition(markerPositions[index, 0], markerPositions[index, 1], gpsLatitude, gpsLongitude);

            GameObject marker = GetMarker(index);

            // Reposition
            // marker.GetComponent<PoiBillboard>().SetPosition(new Vector3(offset.y, 0, offset.x));
            marker.transform.localPosition = new Vector3(offset.y, 0, offset.x);

            // Rescale
            float scale = 1 + (Mathf.Max(offset.x, offset.y) / 50);
            marker.transform.localScale = new Vector3(scale, scale, scale);

            marker.SetActive(true);

            // marker.GetComponent<Marker>().distanceLabel.text = (offset.x) + " | " + (offset.y);
        }

        void OnPlanesChanged(ARPlanesChangedEventArgs args)
        {
            if (args.updated.Count > 0)
            {
                List<Vector3> boundaries = new List<Vector3>();

                ARPlane arPlane = args.updated[0];

                string text = "normal: " + arPlane.normal + " | center: " + arPlane.center;

                // if (arPlane.TryGetBoundary(boundaries))
                // {
                //     foreach (Vector3 v in boundaries)
                //     {
                //         text += "\nvector: " + v;
                //     }

                    m_gpsUIText.GetComponent<TMP_Text>().text = text;

                    if (billboard == null)
                        billboard = Instantiate(photoTemplate, transform, false);

                    billboard.transform.localPosition = new Vector3(arPlane.center.x, 0, arPlane.center.z);
                    billboard.SetActive(true);
                // }
            }
        }

        void OnLocationChanged(float altitude, float gpsLatitude, float gpsLongitude, double timestamp)
        {
            // m_gpsUIText.GetComponent<TMP_Text>().text = "location: " + gpsLatitude + " - " + gpsLongitude + " - " + altitude;

            this.altitude = altitude;

            this.gpsLatitude = gpsLatitude;
            this.gpsLongitude = gpsLongitude;

            ProcessGpsChange();
        }

        void OnCompassChanged(float compassHeading)
        {
            // m_gpsUIText.GetComponent<TMP_Text>().text = "compassHeading: " + compassHeading + "\n";
            
            this.compassHeading = compassHeading;

            // if (compassHeadings.Count == 5)
            //     compassHeadings.Dequeue();
    
            // compassHeadings.Enqueue(compassHeading);

            ProcessCompassChange();
        }

        void ProcessGpsChange()
        {
            GetPOIs();

            for (int i = 0; i < markerPositions.GetLength(0); ++i)
            {
                SetMarker(i);
            }
        }

        void ProcessCompassChange()
        {
            // Experiment of exponential average movement for compass heading, not working great
            // float alpha = 2 / (float)(compassHeadings.Count + 1);
            // var s = compassHeadings.ToArray();
            // float result = 0;

            // string text = "";

            // for (var i = 0; i < compassHeadings.Count; i++)
            // {
            //     result = i == 0 ? s[i] : alpha * s[i] + (1 - alpha) * result;
            //     text += result + "\n";
            // }

            Quaternion cameraRotation = Quaternion.Euler(0, m_mainCamera.localEulerAngles.y, 0);
            Quaternion compass = Quaternion.Euler(0, -compassHeading, 0);

            Quaternion north = Quaternion.Euler(0, cameraRotation.eulerAngles.y + compass.eulerAngles.y, 0);

            float diff = Quaternion.Angle(transform.rotation, north);

            string text = "diff1: " + diff;

            // string text = 
            //             "ARSessionOrigin\n" +
            //             m_mainCamera.parent.eulerAngles + " | " + m_mainCamera.parent.localEulerAngles + "\n" +
            //             m_mainCamera.parent.position + " | " + m_mainCamera.parent.localPosition + "\n\n" +
            //             "ARCamera\n" +
            //             m_mainCamera.eulerAngles + " | " + m_mainCamera.localEulerAngles + "\n" +
            //             m_mainCamera.position + " | " + m_mainCamera.localPosition + "\n\n" +
            //             "WorldBuilder\n" +
            //             transform.eulerAngles + " | " + transform.localEulerAngles + "\n" +
            //             transform.position + " | " + transform.localPosition + "\n\n" +
            //             "North\n" +
            //             compass.eulerAngles;

            // m_gpsUIText.GetComponent<TMP_Text>().text = text;

            // m_gpsUIText.GetComponent<TMP_Text>().text = "camera: " + cameraRotation.eulerAngles.y + "\n" +
            // "compass: " + compass.eulerAngles.y + "\n" + 
            // "north: " + north.eulerAngles.y + "\n" +
            // "diff: " + diff;

            if (Mathf.Abs(diff) > 20f)
            {
                // targetRotation = north;
                // transform.rotation = north;
            }

            // m_gpsUIText.GetComponent<TMP_Text>().text = "true heading: " + -compassHeading + " | " + compass.eulerAngles.y + "\n" +
            //     "camera: " + m_mainCamera.eulerAngles.y + " | " + cameraRotation.eulerAngles.y + "\n" +
            //     "north: " + north.eulerAngles.y;

            // if (m_mainCamera.localEulerAngles.x > 180f || m_mainCamera.localEulerAngles.x < 20f)
            // {
            //     m_gpsUIText.GetComponent<TMP_Text>().text = "local euler angles: " + m_mainCamera.localEulerAngles;
            //     return;
            // }

            float newYRotationAngle = -compassHeading + m_mainCamera.localEulerAngles.y;
            if (newYRotationAngle < 0)
                newYRotationAngle = newYRotationAngle + 360;

            text += "\ndiff2: " + Mathf.Abs(previousYRotationAngle - newYRotationAngle);

            // difference threshold for world rotation
            if (Mathf.Abs(previousYRotationAngle - newYRotationAngle) > 30f) {
                transform.rotation = Quaternion.Euler(0, newYRotationAngle, 0);
                // Quaternion targetRotation = Quaternion.Euler(0, newYRotationAngle, 0);
                // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);
                previousYRotationAngle = newYRotationAngle;
            }

            m_gpsUIText.GetComponent<TMP_Text>().text = text;

            // m_gpsUIText.GetComponent<TMP_Text>().text = "true heading: " + compassHeading + "\n" +
            //     "camera localEulerAngles.y: " + m_mainCamera.transform.localEulerAngles.y + "\n" +
            //     "newYRotationAngle: " + newYRotationAngle;
        }

        public void GetPOIs()
        {
            currentPOIList.Clear();

            StartCoroutine(networkManager.GetPOIs((UnityWebRequest req) =>
            {
                if (req.result == UnityWebRequest.Result.ConnectionError ||
                    req.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log($"{req.error}: {req.downloadHandler.text}");
                }
                else
                {
                    Debug.Log(req.downloadHandler.text);

                    // temporary solution customized for mock API server, because Unity does not support array
                    POIArray poiArray = JsonUtility.FromJson<POIArray>("{\"poiArray\":" + req.downloadHandler.text + "}");

                    for (int i = 0; i < poiArray.poiArray.Length; ++i)
                    {
                        POI poi = poiArray.poiArray[i];

                        Debug.Log(poi.title);

                        currentPOIList.Add(poi);
                    }
                }
            }));
        }
   }
}
