using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace HistocachingII
{
    public class World : MonoBehaviour
    {
        public TMP_Text m_DebugText1;
        public TMP_Text m_DebugText2;

        private Camera m_MainCamera;

        public GameObject markerTemplate;
        public GameObject photoTemplate;
        public GameObject histocacheLinePrefab;
        public GameObject histocachingSpotPrefab;

        private Histocache[] histocacheCollection;

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
            Histocache histocache = null;
            // Histocache histocache = DataManager.Instance.GetHistocacheCollection()[index];

            GameObject marker = GetMarker(index, name);

            Vector2 offset = Conversions.GeoToUnityPosition(histocache.lat, histocache.@long, (float) gpsLatitude, (float) gpsLongitude);
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
                // if (marker.name == "60ba450fb296fa521956bd15")
                if (marker.name == "61114a1f084fe30bc9140582")
                {
                    // float histocachingSpotPositionLat = 51.02691327388887f;
                    // float histocachingSpotPositionLong = 13.725401096277498f;
                    float histocachingSpotPositionLat = 51.03169002104799f;
                    float histocachingSpotPositionLong = 13.72475585100041f;
                    Vector2 spotOffset = Conversions.GeoToUnityPosition(histocachingSpotPositionLat, histocachingSpotPositionLong, (float) gpsLatitude, (float) gpsLongitude);
                    Vector3 histocachingSpotPosition = new Vector3(spotOffset.y, 0.0f, spotOffset.x);
                    // Vector3 histocachingSpotPosition = new Vector3(0.0f, 0.0f, 3.0f);

                    // float photoPositionLat = 51.026989314055f;
                    // float photoPositionLong = 13.725200653079778f;
                    float photoPositionLat = 51.03179797712443f;
                    float photoPositionLong = 13.72475048658238f;
                    Vector2 photoOffset = Conversions.GeoToUnityPosition(photoPositionLat, photoPositionLong, (float) gpsLatitude, (float) gpsLongitude);
                    Vector3 photoPosition = new Vector3(photoOffset.y, 0.0f, photoOffset.x);

                    GameObject histocacheLine = Instantiate(histocacheLinePrefab, transform, false);
                    var points = new Vector3[2]; 
                    points[0] = histocachingSpotPosition;
                    points[1] = marker.transform.localPosition;
                    histocacheLine.GetComponent<HistocacheLine>().SetPositions(points);

                    GameObject histocachingSpot = Instantiate(histocachingSpotPrefab, transform, false);
                    histocachingSpot.transform.localPosition = histocachingSpotPosition;
                    histocachingSpot.transform.LookAt(marker.transform.position);

                    if (m_POIPhoto == null)
                        m_POIPhoto = Instantiate(photoTemplate, transform, false);

                    // Vector3 direction = marker.transform.position - histocachingSpot.transform.position;

                    // m_POIPhoto.transform.localPosition = histocachingSpot.transform.position + 2f * direction.normalized;
                    m_POIPhoto.transform.localPosition = photoPosition;
                    // m_POIPhoto.transform.LookAt(histocachingSpot.transform.position);

                    // Vector3 lookPosition = m_POIPhoto.transform.position - histocachingSpot.transform.position;
                    // lookPosition.y = 0;
                    // m_POIPhoto.transform.rotation = Quaternion.LookRotation(lookPosition);
                    
                    if (string.IsNullOrWhiteSpace(histocache.image_url))
                    {
                        GetPOIDocument((POI p) => {

                            if (p != null)
                            {
                                histocache.image_url = p.image_url;
                                histocache.image_height = p.image_height;
                                histocache.image_aspect_ratio = p.image_aspect_ratio;
                                histocache.title_de = p.title_de;
                                histocache.title_en = p.title_en;
                                histocache.description_de = p.description_de;
                                histocache.description_en = p.description_en;
                                histocache.caption_de = p.caption_de;
                                histocache.caption_en = p.caption_en;

                                // histocache.documents = p.documents;

                                // DataManager.Instance.GetMutableHistocacheCollection()[index] = histocache;

                                // m_POIPhoto.GetComponent<POIPhoto>().SetPhotoURL(poi.image_url, poi.image_aspect_ratio);
                                m_POIPhoto.GetComponent<POIPhoto>().SetPhotoURL("https://hcii-cms.omdat.id/storage/pois/60ba450fb296fa521956bd15/80b5d02e73436cd1645d7f8781730bc9.png", histocache.image_aspect_ratio);

                                m_POITitle.text = m_LanguageToggle.isOn ? histocache.title_en : histocache.title_de;

                                m_POIButton.onClick.RemoveAllListeners();
                                m_POIButton.onClick.AddListener(() => OnPOI(index));

                                m_POIButton.gameObject.SetActive(true);
                            }

                        }, histocache.id);
                    }
                    else
                    {
                        // m_POIPhoto.GetComponent<POIPhoto>().SetPhotoURL(poi.image_url, poi.image_aspect_ratio);
                        m_POIPhoto.GetComponent<POIPhoto>().SetPhotoURL("https://hcii-cms.omdat.id/storage/pois/60ba450fb296fa521956bd15/80b5d02e73436cd1645d7f8781730bc9.png", histocache.image_aspect_ratio);

                        m_POITitle.text = m_LanguageToggle.isOn ? histocache.title_en : histocache.title_de;

                        m_POIButton.onClick.RemoveAllListeners();
                        m_POIButton.onClick.AddListener(() => OnPOI(index));

                        m_POIButton.gameObject.SetActive(true);
                    }
                }

                // marker.GetComponent<Marker>().distanceLabel.text = (offset.x) + " | " + (offset.y);
            // }
            // else
            // {
            //     if (marker.activeSelf)
            //         marker.SetActive(false);
            // }
        }

        private void GetPOICollection()
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
                if (!( child.name == "Compass" || child.name == "Cube"))
                    GameObject.Destroy(child.gameObject);

            m_POIPhoto = null;

            // for (int i = 0; i < markers.Count; ++i)
            // {
            //     GameObject gameObject = markers[i];
            //     gameObject.Destroy();
            // }

            markers.Clear();

            // this.poiCollection.Clear();

            // if (data.histocacheCollection.Count == 0)
            //     data.FetchPoiCollection();

            DataManager.Instance.GetHistocacheCollection((Histocache[] histocacheCollection) =>
            {
                this.histocacheCollection = histocacheCollection;

                int index = 0;
                foreach (Histocache histocache in histocacheCollection)
                {
                    SetMarker(index++, histocache.id);
                }
            });

            // for (int i = 0; i < m_Data.histocacheCollection.Count; ++i)
            // {
            //     POI poi = m_Data.histocacheCollection[i];

            //     this.poiCollection.Add(poi);

            //     SetMarker(i, poi.id);
            // }

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

            StartCoroutine(NetworkManager.GetPOIDocument((POI poi) =>
            {
                m_IsLoadingPOIDocument = false;

                callback(poi);

                // m_DebugText2.text += "GetPOIDocument end (" + poi?.image_url + ")\n";

            }, poiId));
        }

        void OnPOI(int index)
        {
            // poiDetail.Show(m_LanguageToggle.isOn ? 1 : 0, poiCollection[index]);
        }

        public void SetLatestTargetRotation(Quaternion targetRotation)
        {
            m_LatestTargetRotation = targetRotation;
        }
    }
}
