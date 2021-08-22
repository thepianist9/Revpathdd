using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace HistocachingII
{
    public class WorldBuilder : MonoBehaviour
    {
        /************************************************
         * Plugin test
         *
         * Code in h file:
         * #pragma once
         * #if UNITY_METRO
         * #define EXPORT_API __declspec(dllexport) __stdcall
         * #elif UNITY_WIN
         * #define EXPORT_API __declspec(dllexport)
         * #else
         * #define EXPORT_API
         * #endif
         * 
         * Code in cpp file:
         * #include <stdlib.h>
         * #include "LowLevelPlugin.hpp"
         *
         * extern "C" int EXPORT_API getInt() {
         *    return 12345;
         * }
         ***********************************************/
        /*
        #if UNITY_IPHONE
            // On iOS plugins are statically linked into
            // the executable, so we have to use __Internal as the
            // library name.
            [DllImport ("__Internal")]
        #else
        // Other platforms load plugins dynamically, so pass the name
        // of the plugin's dynamic library.
            [DllImport ("LowLevelPlugin")]
        #endif
        
        private static extern int getInt();

        void PluginTest()
        {
            Debug.Log("PluginTest " + getInt());
        }
        //*/

        private List<POI> poiCollection = new List<POI>();

        private Camera m_MainCamera;

        public LocationService locationService;
        // public CompassService compassService;

        public GameObject m_gpsUIText;

        public GameObject markerTemplate;
        public GameObject photoTemplate;

        private List<GameObject> markers = new List<GameObject>();

        private GameObject m_HistocachePhoto;

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

        public TMP_Text _tmpText;

        private bool m_IsLoadingPOI = false;
        private bool m_IsLoadingPOIDocument = false;

        public GameObject m_POIPanel;
        public Text m_POITitle;
        public Text m_POICaption;

        void Awake()
        {
            // cameraRotation = m_MainCamera.rotation.eulerAngles;

            // Data data = new Data();
            // markerPositions = data.poiLocations;

            targetRotation = transform.rotation;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_MainCamera = Camera.main;

            locationService.locationChangedEvent.AddListener(OnLocationChanged);
            locationService.headingChangedEvent.AddListener(OnHeadingChanged);

            // compassService.compassChangedEvent.AddListener(OnKompassChanged);

            // GetPOICollection();
        }

        void Destroy()
        {
            locationService.locationChangedEvent.RemoveListener(OnLocationChanged);
            locationService.headingChangedEvent.RemoveListener(OnHeadingChanged);

            // compassService.compassChangedEvent.RemoveListener(OnKompassChanged);
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = m_MainCamera.transform.localPosition;
            // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);

            if (Input.deviceOrientation != DeviceOrientation.Portrait)
                return;

            // TODO: move this somewhere else and only update every 0.5 second
            int count = 0;
            GameObject m = null;
            int index = 0;

            string text = "";

            for (int i = 0; i < markers.Count; ++i)
            {
                GameObject gameObject = markers[i];

                Vector3 target = gameObject.transform.position - transform.position;
                float angle = Vector3.Angle(target, m_MainCamera.transform.forward);

                // TODO: find real FOV calculation
                if (angle <= 30) // 60° FOV
                {
                    // m_gpsUIText.GetComponent<TMP_Text>().text = "camera: " + m_MainCamera.transform.forward + " | target: " + target;

                    count += 1;
                    m = gameObject;
                    index = i;

                    text += i + " " + gameObject.transform.position + " " + gameObject.transform.localPosition + "\n";
                }
            }

            // m_gpsUIText.GetComponent<TMP_Text>().text = "fov: " + text + "\n";

            if (count == 1)
            {
                // TODO
                // if (m.GetComponent<POIBillboard>().GetSquaredDistance() <= 400) // squared distance is less than 400 m
                // if (m.transform.localPosition.x * m.transform.localPosition.x + m.transform.localPosition.z * m.transform.localPosition.z <= 400)
                {
                    if (m_HistocachePhoto == null)
                        m_HistocachePhoto = Instantiate(photoTemplate, transform, false);

                    m_HistocachePhoto.transform.localPosition = new Vector3(m.transform.localPosition.x, 0, m.transform.localPosition.z);
                    m_HistocachePhoto.SetActive(true);

                    // Vector3 forward = m_MainCamera.transform.position - m_HistocachePhoto.transform.position;
                    // m_HistocachePhoto.transform.Translate(forward * 0.1f);

                    POI poi = poiCollection[index];

                    m_POITitle.text = poi.title_de;
                    m_POICaption.text = poi.description_de;
                    m_POIPanel.SetActive(true);

                    if (string.IsNullOrWhiteSpace(poi.image_url))
                    {
                        GetPOIDocument((POI p) => {

                            if (p != null)
                            {
                                poi.image_url = p.image_url;
                                poi.image_height = p.image_height;
                                poi.title_de = p.title_de;
                                poi.title_en = p.title_en;
                                poi.description_de = p.description_de;
                                poi.description_en = p.description_en;
                                poi.caption_de = p.caption_de;
                                poi.caption_en = p.caption_en;

                                poiCollection[index] = poi;

                                m_HistocachePhoto.GetComponent<HistocachePhoto>().SetPhotoURL(poi.image_url, 15f, 1, transform);
                            }

                        }, poi.id);
                    }
                    else
                    {
                        m_HistocachePhoto.GetComponent<HistocachePhoto>().SetPhotoURL(poi.image_url, 15f, 1, transform);
                    }
                }
            }
            else
            {
                if (m_HistocachePhoto)
                    m_HistocachePhoto.SetActive(false);

                m_POIPanel.SetActive(false);
            }
        }

        GameObject GetMarker(int index)
        {
            if (markers.Count < index + 1)
            {
                GameObject marker = Instantiate(markerTemplate, transform, false);
                //marker.GetComponent<POIBillboard>().POIClickedEvent.AddListener(OnPoiClicked);

                markers.Add(marker);
            }

            return markers[index];
        }

        void SetMarker(int index)
        {
            POI poi = poiCollection[index];

            Vector2 offset = Conversions.GeoToUnityPosition(poi.lat, poi.@long, gpsLatitude, gpsLongitude);
            if (offset.x > m_MainCamera.farClipPlane)
                return;

            GameObject marker = GetMarker(index);

            // marker.GetComponent<POIBillboard>().SetId(poi.id);

            // Reposition
            // marker.GetComponent<POIBillboard>().SetPosition(new Vector3(offset.y, 0, offset.x));
            marker.transform.localPosition = new Vector3(offset.y, 0, offset.x);

            // Rescale
            float scale = 1 + (Mathf.Max(offset.x, offset.y) / 50);
            marker.transform.localScale = new Vector3(scale, scale, scale);

            marker.SetActive(true);

            // marker.GetComponent<Marker>().distanceLabel.text = (offset.x) + " | " + (offset.y);
        }

        void OnLocationChanged(float altitude, float gpsLatitude, float gpsLongitude, double timestamp)
        {
            // m_gpsUIText.GetComponent<TMP_Text>().text = "location: " + gpsLatitude + " - " + gpsLongitude + " - " + altitude;

            this.altitude = altitude;

            this.gpsLatitude = gpsLatitude;
            this.gpsLongitude = gpsLongitude;

            ProcessGpsChange();
        }

        void OnHeadingChanged(float compassHeading)
        {
            // m_gpsUIText.GetComponent<TMP_Text>().text = "compassHeading: " + compassHeading + "\n";
            
            this.compassHeading = compassHeading;

            // if (compassHeadings.Count == 5)
            //     compassHeadings.Dequeue();
    
            // compassHeadings.Enqueue(compassHeading);

            ProcessCompassChange();
        }

        // void OnKompassChanged(float compassHeading)
        // {
        //     this.compassHeading = compassHeading;

        //     ProcessCompassChange();
        // }

        void OnPOIClicked(string id)
        {
            // GetPOIDocument(id);
        }

        void ProcessGpsChange()
        {
            GetPOICollection();
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

            Quaternion cameraRotation = Quaternion.Euler(0, m_MainCamera.transform.localEulerAngles.y, 0);
            Quaternion compass = Quaternion.Euler(0, -compassHeading, 0);

            Quaternion north = Quaternion.Euler(0, cameraRotation.eulerAngles.y + compass.eulerAngles.y, 0);

            float diff = Quaternion.Angle(transform.rotation, north);

            string text = "diff1: " + diff;

            // string text = 
            //             "ARSessionOrigin\n" +
            //             m_MainCamera.transform.parent.eulerAngles + " | " + m_MainCamera.transform.parent.localEulerAngles + "\n" +
            //             m_MainCamera.transform.parent.position + " | " + m_MainCamera.transform.parent.localPosition + "\n\n" +
            //             "ARCamera\n" +
            //             m_MainCamera.transform.eulerAngles + " | " + m_MainCamera.transform.localEulerAngles + "\n" +
            //             m_MainCamera.transform.position + " | " + m_MainCamera.transform.localPosition + "\n\n" +
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
            //     "camera: " + m_MainCamera.transform.eulerAngles.y + " | " + cameraRotation.eulerAngles.y + "\n" +
            //     "north: " + north.eulerAngles.y;

            // if (m_MainCamera.transform.localEulerAngles.x > 180f || m_MainCamera.transform.localEulerAngles.x < 20f)
            // {
            //     m_gpsUIText.GetComponent<TMP_Text>().text = "local euler angles: " + m_MainCamera.transform.localEulerAngles;
            //     return;
            // }

            float newYRotationAngle = -compassHeading + m_MainCamera.transform.localEulerAngles.y;
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

            // m_gpsUIText.GetComponent<TMP_Text>().text = text;

            // m_gpsUIText.GetComponent<TMP_Text>().text = "true heading: " + compassHeading + "\n" +
            //     "camera localEulerAngles.y: " + m_MainCamera.transform.transform.localEulerAngles.y + "\n" +
            //     "newYRotationAngle: " + newYRotationAngle;

            m_gpsUIText.GetComponent<TMP_Text>().text = "Compass heading " + compassHeading;
        }

        public void GetPOICollection()
        {
            if (m_IsLoadingPOI)
                return;

            m_IsLoadingPOI = true;

            _tmpText.text += "GetPOICollection begin\n";

            this.poiCollection.Clear();

            StartCoroutine(NetworkManager.GetPOICollection((POI[] poiCollection) =>
            {
                // m_IsLoadingPOI = false;

                for (int i = 0; i < poiCollection?.Length; ++i)
                {
                    POI poi = poiCollection[i];

                    this.poiCollection.Add(poi);
                }

                _tmpText.text += "GetPOICollection end (" + poiCollection?.Length + " places)\n";

                for (int i = 0; i < this.poiCollection.Count; ++i)
                {
                    POI poi = this.poiCollection[i];

                    SetMarker(i);
                }
            }));
        }

        public void GetPOIDocument(Action<POI> callback, string poiId)
        {
            if (m_IsLoadingPOIDocument)
                return;

            m_IsLoadingPOIDocument = true;

            _tmpText.text += "GetPOIDocument begin\n";

            StartCoroutine(NetworkManager.GetPOIDocument((POI poi) =>
            {
                m_IsLoadingPOIDocument = false;

                callback(poi);

                _tmpText.text += "GetPOIDocument end (" + poi?.image_url + ")\n";

            }, poiId));
        }
   }
}
