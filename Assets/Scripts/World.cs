using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace HistocachingII
{
    public class World : MonoBehaviour
    {
        public TMP_Text m_DebugText1;
        public TMP_Text m_DebugText2;

        public Data m_Data;

        private NetworkManager networkManager = new NetworkManager();

        private List<POI> poiCollection = new List<POI>();

        private Camera m_MainCamera;

        public GameObject markerTemplate;
        public GameObject photoTemplate;
        public GameObject histocacheLinePrefab;
        public GameObject histocachingSpotPrefab;

        private List<GameObject> markers = new List<GameObject>();

        private GameObject m_POIPhoto = null;

        private bool m_IsLoadingPOI = false;
        private bool m_IsLoadingPOIDocument = false;

        public Button m_POIButton;
        public Text m_POITitle;

        public Toggle m_LanguageToggle;

        public Documents poiDetail;

        // Location
        private double gpsLatitude = float.MinValue;
        private double gpsLongitude = float.MinValue;

        private bool m_IsWorldRotated;
        Quaternion m_LatestTargetRotation;

        ILocationProvider _locationProvider;
		ILocationProvider LocationProvider
		{
			get
			{
				if (_locationProvider == null)
				{
					_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
				}

				return _locationProvider;
			}
		}

        void Start()
        {
            m_MainCamera = Camera.main;
            m_IsWorldRotated = false;
        }

        void Update()
        {
            // m_DebugText1.text = "DeviceOrientation = " + Input.gyro.attitude.eulerAngles.x;
        }

        public void GenerateWorld()
        {
            // Vector3 targetPosition = m_MainCamera.transform.position;
			// targetPosition.y -= 1.8f;
			// transform.position = targetPosition;

            // if (!m_IsWorldRotated)
            // {
                transform.localRotation = m_LatestTargetRotation;
            //     m_IsWorldRotated = true;
            // }

            gpsLatitude = LocationProvider.CurrentLocation.LatitudeLongitude.x;
            gpsLongitude = LocationProvider.CurrentLocation.LatitudeLongitude.y;

            GetPOICollection();
        }

        GameObject GetMarker(int index, String name)
        {
            // if (markers.Count < index + 1)
            // {
                GameObject marker = Instantiate(markerTemplate, transform, false);
                // GameObject marker = Instantiate(photoTemplate, transform, false);
                //marker.GetComponent<POIBillboard>().POIClickedEvent.AddListener(OnPoiClicked);

                marker.name = name;

                markers.Add(marker);
            // }

            return markers[index];
        }

        void SetMarker(int index, String name)
        {
            POI poi = poiCollection[index];

            GameObject marker = GetMarker(index, name);

            Vector2 offset = Conversions.GeoToUnityPosition(poi.lat, poi.@long, (float) gpsLatitude, (float) gpsLongitude);
            // if (offset.x < m_MainCamera.farClipPlane)
            // {
                // marker.GetComponent<POIBillboard>().SetId(poi.id);

                // Reposition
                // marker.GetComponent<POIBillboard>().SetPosition(new Vector3(offset.y, 0, offset.x));
                marker.transform.localPosition = new Vector3(offset.y, 0, offset.x);

                // Rescale
                float scale = 1 + (Mathf.Max(offset.x, offset.y) / 50);
                marker.transform.localScale = new Vector3(scale, scale, scale);

                if (!marker.activeSelf)
                    marker.SetActive(true);

                // TODO: temporary solution, need to change database, data, and cms
                if (marker.name == "60ba450fb296fa521956bd15")
                {
                    float histocachingSpotPositionLat = 51.02696050957119f;
                    float histocachingSpotPositionLong = 13.725438647203706f;
                    offset = Conversions.GeoToUnityPosition(histocachingSpotPositionLat, histocachingSpotPositionLong, (float) gpsLatitude, (float) gpsLongitude);
                    Vector3 histocachingSpotPosition = new Vector3(offset.y, 0.0f, offset.x);

                    GameObject histocacheLine = Instantiate(histocacheLinePrefab, transform, false);
                    var points = new Vector3[2]; 
                    points[0] = new Vector3(0.0f, 0.0f, 3.0f);
                    points[1] = marker.transform.localPosition;
                    histocacheLine.GetComponent<HistocacheLine>().SetPositions(points);

                    GameObject histocachingSpot = Instantiate(histocachingSpotPrefab, transform, false);
                    histocachingSpot.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);
                    histocachingSpot.transform.LookAt(marker.transform.position);

                    if (m_POIPhoto == null)
                        m_POIPhoto = Instantiate(photoTemplate, transform, false);

                    m_POIPhoto.transform.localPosition = new Vector3(offset.x, 0.0f, offset.y);
                    m_POIPhoto.transform.LookAt(histocachingSpot.transform.localPosition);
                    
                    if (string.IsNullOrWhiteSpace(poi.image_url))
                    {
                        GetPOIDocument((POI p) => {

                            if (p != null)
                            {
                                poi.image_url = p.image_url;
                                poi.image_height = p.image_height;
                                poi.image_aspect_ratio = p.image_aspect_ratio;
                                poi.title_de = p.title_de;
                                poi.title_en = p.title_en;
                                poi.description_de = p.description_de;
                                poi.description_en = p.description_en;
                                poi.caption_de = p.caption_de;
                                poi.caption_en = p.caption_en;

                                poi.documents = p.documents;

                                poiCollection[index] = poi;

                                m_POIPhoto.GetComponent<POIPhoto>().SetPhotoURL(poi.image_url, poi.image_aspect_ratio);
                            }

                        }, poi.id);
                    }
                    else
                    {
                        m_POIPhoto.GetComponent<POIPhoto>().SetPhotoURL(poi.image_url, poi.image_aspect_ratio);
                    }

                    m_POITitle.text = m_LanguageToggle.isOn ? poi.title_en : poi.title_de;
                    
                    m_POIButton.onClick.RemoveAllListeners();
                    m_POIButton.onClick.AddListener(() => OnPOI(index));

                    // m_POIButton.gameObject.SetActive(true);
                }

                // marker.GetComponent<Marker>().distanceLabel.text = (offset.x) + " | " + (offset.y);
            // }
            // else
            // {
            //     if (marker.activeSelf)
            //         marker.SetActive(false);
            // }
        }

        public void GetPOICollection()
        {
            // if (m_IsLoadingPOI)
            // {
            //     for (int i = 0; i < this.poiCollection?.Count; ++i)
            //     {
            //         POI poi = this.poiCollection[i];

            //         SetMarker(i);
            //     }

            //     return;
            // }

            // m_IsLoadingPOI = true;

            // m_DebugText2.text += "GetPOICollection begin\n";

            foreach (Transform child in transform)
                if (child.name != "Compass")
                    GameObject.Destroy(child.gameObject);

            m_POIPhoto = null;

            // for (int i = 0; i < markers.Count; ++i)
            // {
            //     GameObject gameObject = markers[i];
            //     gameObject.Destroy();
            // }

            markers.Clear();

            this.poiCollection.Clear();

            // if (data.histocacheCollection.Count == 0)
            //     data.FetchPoiCollection();

            for (int i = 0; i < m_Data.histocacheCollection.Count; ++i)
            {
                POI poi = m_Data.histocacheCollection[i];

                this.poiCollection.Add(poi);

                SetMarker(i, poi.id);
            }

            // StartCoroutine(networkManager.GetPOICollection((POI[] poiCollection) =>
            // {
            //     // m_IsLoadingPOI = false;

            //     for (int i = 0; i < poiCollection?.Length; ++i)
            //     {
            //         POI poi = poiCollection[i];

            //         this.poiCollection.Add(poi);
            //     }

            //     // m_DebugText2.text += "GetPOICollection end (" + poiCollection?.Length + " places)\n";

            //     for (int i = 0; i < this.poiCollection.Count; ++i)
            //     {
            //         POI poi = this.poiCollection[i];

            //         SetMarker(i);
            //     }
            // }));
        }

        public void GetPOIDocument(Action<POI> callback, string poiId)
        {
            if (m_IsLoadingPOIDocument)
                return;

            m_IsLoadingPOIDocument = true;

            // m_DebugText2.text += "GetPOIDocument begin\n";

            StartCoroutine(networkManager.GetPOIDocument((POI poi) =>
            {
                m_IsLoadingPOIDocument = false;

                callback(poi);

                // m_DebugText2.text += "GetPOIDocument end (" + poi?.image_url + ")\n";

            }, poiId));
        }

        void OnPOI(int index)
        {
            poiDetail.Show(m_LanguageToggle.isOn ? 1 : 0, poiCollection[index]);
        }

        public void SetLatestTargetRotation(Quaternion targetRotation)
        {
            m_LatestTargetRotation = targetRotation;
        }
    }
}
