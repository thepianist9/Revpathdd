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

        private NetworkManager networkManager = new NetworkManager();

        private List<POI> poiCollection = new List<POI>();

        private Camera m_MainCamera;

        public GameObject markerTemplate;
        public GameObject photoTemplate;

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
            m_DebugText1.text = "localRotation = " + transform.localRotation + "\n"
                + "rotation = " + transform.rotation + "\n"
                + "localPosition = " + transform.localPosition + "\n"
                + "position = " + transform.position + "\n";
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

        GameObject GetMarker(int index)
        {
            // if (markers.Count < index + 1)
            // {
                GameObject marker = Instantiate(markerTemplate, transform, false);
                // GameObject marker = Instantiate(photoTemplate, transform, false);
                //marker.GetComponent<POIBillboard>().POIClickedEvent.AddListener(OnPoiClicked);

                markers.Add(marker);
            // }

            return markers[index];
        }

        void SetMarker(int index)
        {
            POI poi = poiCollection[index];

            GameObject marker = GetMarker(index);

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

            this.poiCollection.Clear();

            for (int i = 0; i < markers.Count; ++i)
            {
                GameObject gameObject = markers[i];
                gameObject.Destroy();
            }

            markers.Clear();

            StartCoroutine(networkManager.GetPOICollection((POI[] poiCollection) =>
            {
                // m_IsLoadingPOI = false;

                for (int i = 0; i < poiCollection?.Length; ++i)
                {
                    POI poi = poiCollection[i];

                    this.poiCollection.Add(poi);
                }

                // m_DebugText2.text += "GetPOICollection end (" + poiCollection?.Length + " places)\n";

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
