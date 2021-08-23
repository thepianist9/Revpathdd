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
        private Camera m_MainCamera;

        public GameObject locationTemplate;
        public GameObject viewpointTemplate;
        public GameObject photoTemplate;
        public GameObject lineTemplate;

        private Dictionary<string, Histocache> histocacheCollection = new Dictionary<string, Histocache>();

        private Dictionary<string, GameObject> histocacheMarkers = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> viewpointMarkers = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> photos = new Dictionary<string, GameObject>();

        private GameObject m_Viewpoint = null;
        private GameObject m_HistocachePhoto = null;

        private bool m_IsLoadingPOI = false;
        private bool m_IsLoadingPOIDocument = false;

        public Button m_DetailBtn;
        public Text m_DetailBtnLabel;

        public Toggle m_LanguageToggle;

        public Documents documents;

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

        // void Update()
        // {
        //     m_DebugText1.text = "DeviceOrientation = " + Input.gyro.attitude.eulerAngles.x;
        // }

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

            GetHistocacheCollection();
        }

        GameObject GetHistocacheMarker(string histocacheId)
        {
            GameObject marker;

            if (!histocacheMarkers.TryGetValue(histocacheId, out marker))
            {
                marker = Instantiate(locationTemplate, transform, false);
                //marker.GetComponent<POIBillboard>().POIClickedEvent.AddListener(OnPoiClicked);

                histocacheMarkers.Add(histocacheId, marker);
            }

            return marker;
        }

        GameObject GetViewpointMarker(string histocacheId)
        {
            GameObject marker;

            if (!viewpointMarkers.TryGetValue(histocacheId, out marker))
            {
                marker = Instantiate(viewpointTemplate, transform, false);
                //marker.GetComponent<POIBillboard>().POIClickedEvent.AddListener(OnPoiClicked);

                viewpointMarkers.Add(histocacheId, marker);
            }

            return marker;
        }

        HistocachePhoto GetPhoto(string histocacheId)
        {
            GameObject photo;

            if (!photos.TryGetValue(histocacheId, out photo))
            {
                photo = Instantiate(photoTemplate, transform, false);

                photos.Add(histocacheId, photo);
            }

            return photo.GetComponent<HistocachePhoto>();
        }

        void SetMarker(Histocache histocache)
        {
            if (!histocache.has_histocache_location)
                return;
            
            Vector2 offset = Conversions.GeoToUnityPosition(histocache.lat, histocache.@long, (float) gpsLatitude, (float) gpsLongitude);
            // if (offset.x < m_MainCamera.farClipPlane)
            {
                GameObject marker = GetHistocacheMarker(histocache._id);

                // marker.GetComponent<POIBillboard>().SetId(poi.id);

                // Reposition
                // marker.GetComponent<POIBillboard>().SetPosition(new Vector3(offset.y, 0, offset.x));
                marker.transform.localPosition = new Vector3(offset.y, 0, offset.x);

                // Rescale
                float scale = 1f + (Mathf.Max(offset.x, offset.y) / 50f);
                marker.transform.localScale = new Vector3(scale, scale, scale);

                if (!marker.activeSelf)
                    marker.SetActive(true);

                if (!histocache.has_viewpoint_location)
                    return;

                {
                    // Viewpoint
                    GameObject viewpointMarker = GetViewpointMarker(histocache._id);

                    Vector2 viewpointOffset = Conversions.GeoToUnityPosition(histocache.viewpoint_lat, histocache.viewpoint_long, (float) gpsLatitude, (float) gpsLongitude);

                    if (histocache._id == "61114a1f084fe30bc9140582")
                        viewpointMarker.transform.localPosition = new Vector3(0, 0, 0);
                    else
                        viewpointMarker.transform.localPosition = new Vector3(viewpointOffset.y, 0, viewpointOffset.x);
                    viewpointMarker.transform.LookAt(marker.transform.position);

                    // Histocache line
                    GameObject histocacheLine = Instantiate(lineTemplate, transform, false);
                    var points = new Vector3[2] { viewpointMarker.transform.localPosition, marker.transform.localPosition};
                    histocacheLine.GetComponent<HistocacheLine>().SetPositions(points);

                    // Photo
                    HistocachePhoto photo = GetPhoto(histocache._id); 
                    photo.transform.localPosition = marker.transform.localPosition;
                    photo.transform.LookAt(viewpointMarker.transform.position);
                    
                    if (string.IsNullOrWhiteSpace(histocache.viewpoint_image_url))
                    {
                        GetHistocache(histocache._id, (Histocache h) =>
                        {
                            if (h != null)
                            {
                                histocache.image_url = h.image_url;
                                histocache.image_aspect_ratio = h.image_aspect_ratio;
                                histocache.title_de = h.title_de;
                                histocache.title_en = h.title_en;
                                histocache.description_de = h.description_de;
                                histocache.description_en = h.description_en;
                                histocache.caption_de = h.caption_de;
                                histocache.caption_en = h.caption_en;

                                histocache.viewpoint_image_url = h.viewpoint_image_url;
                                histocache.viewpoint_image_aspect_ratio = h.viewpoint_image_aspect_ratio;
                                histocache.viewpoint_image_height = h.viewpoint_image_height;
                                histocache.viewpoint_image_offset = h.viewpoint_image_offset;

                                histocache.documents = h.documents;

                                histocacheCollection[histocache._id] = histocache;

                                photo.SetPhotoURL(
                                    histocache.viewpoint_image_url,
                                    histocache.viewpoint_image_height,
                                    histocache.viewpoint_image_aspect_ratio,
                                    histocache.viewpoint_image_offset
                                );

                                m_DetailBtnLabel.text = m_LanguageToggle.isOn ? histocache.title_en : histocache.title_de;

                                m_DetailBtn.onClick.RemoveAllListeners();
                                m_DetailBtn.onClick.AddListener(() => OnPOI(histocache._id));

                                m_DetailBtn.gameObject.SetActive(true);
                            }
                        });
                    }
                    else
                    {
                        photo.SetPhotoURL(
                            histocache.viewpoint_image_url,
                            histocache.viewpoint_image_height,
                            histocache.viewpoint_image_aspect_ratio,
                            histocache.viewpoint_image_offset
                        );

                        m_DetailBtnLabel.text = m_LanguageToggle.isOn ? histocache.title_en : histocache.title_de;

                        m_DetailBtn.onClick.RemoveAllListeners();
                        m_DetailBtn.onClick.AddListener(() => OnPOI(histocache._id));

                        m_DetailBtn.gameObject.SetActive(true);
                    }
                }

                // TODO: temporary solution, need to change database, data, and cms
                // if (marker.name == "61114a1f084fe30bc9140582")
                // {
                //     // float histocachingSpotPositionLat = 51.02691327388887f;
                //     // float histocachingSpotPositionLong = 13.725401096277498f;
                //     float histocachingSpotPositionLat = 51.03169002104799f;
                //     float histocachingSpotPositionLong = 13.72475585100041f;
                //     Vector2 spotOffset = Conversions.GeoToUnityPosition(histocachingSpotPositionLat, histocachingSpotPositionLong, (float) gpsLatitude, (float) gpsLongitude);
                //     Vector3 histocachingSpotPosition = new Vector3(spotOffset.y, 0.0f, spotOffset.x);
                //     // Vector3 histocachingSpotPosition = new Vector3(0.0f, 0.0f, 3.0f);

                //     // float photoPositionLat = 51.026989314055f;
                //     // float photoPositionLong = 13.725200653079778f;
                //     float photoPositionLat = 51.03179797712443f;
                //     float photoPositionLong = 13.72475048658238f;
                //     Vector2 photoOffset = Conversions.GeoToUnityPosition(photoPositionLat, photoPositionLong, (float) gpsLatitude, (float) gpsLongitude);
                //     // Vector3 photoPosition = new Vector3(photoOffset.y, 0.0f, photoOffset.x);
                //     Vector3 photoPosition = new Vector3(0.0f, 0.0f, -2.0f);

                //     GameObject histocacheLine = Instantiate(histocacheLinePrefab, transform, false);
                //     var points = new Vector3[2]; 
                //     points[0] = histocachingSpotPosition;
                //     points[1] = marker.transform.localPosition;
                //     histocacheLine.GetComponent<HistocacheLine>().SetPositions(points);

                //     GameObject histocachingSpot = Instantiate(histocachingSpotPrefab, transform, false);
                //     histocachingSpot.transform.localPosition = histocachingSpotPosition;
                //     histocachingSpot.transform.LookAt(marker.transform.position);

                //     if (m_HistocachePhoto == null)
                //         m_HistocachePhoto = Instantiate(photoTemplate, transform, false);

                //     // Vector3 direction = marker.transform.position - histocachingSpot.transform.position;

                //     // m_HistocachePhoto.transform.localPosition = histocachingSpot.transform.position + 2f * direction.normalized;
                //     m_HistocachePhoto.transform.localPosition = photoPosition;
                //     // m_HistocachePhoto.transform.LookAt(histocachingSpot.transform.position);

                //     // Vector3 histocachePhotoLookPosition = m_MainCamera.transform.position - m_HistocachePhoto.transform.position;
                //     // histocachePhotoLookPosition.y = 0;
                //     // m_HistocachePhoto.transform.rotation = Quaternion.LookRotation(histocachePhotoLookPosition);

                //     // Vector3 lookPosition = m_HistocachePhoto.transform.position - histocachingSpot.transform.position;
                //     // lookPosition.y = 0;
                //     // m_HistocachePhoto.transform.rotation = Quaternion.LookRotation(lookPosition);
                    
                //     if (string.IsNullOrWhiteSpace(histocache.image_url))
                //     {
                //         GetHistocache(histocache.id, (Histocache h) =>
                //         {
                //             if (h != null)
                //             {
                //                 histocache.image_url = h.image_url;
                //                 histocache.image_height = h.image_height;
                //                 histocache.image_aspect_ratio = h.image_aspect_ratio;
                //                 histocache.title_de = h.title_de;
                //                 histocache.title_en = h.title_en;
                //                 histocache.description_de = h.description_de;
                //                 histocache.description_en = h.description_en;
                //                 histocache.caption_de = h.caption_de;
                //                 histocache.caption_en = h.caption_en;

                //                 histocache.documents = h.documents;

                //                 m_HistocachePhoto.GetComponent<HistocachePhoto>().SetPhotoURL(
                //                     "https://hcii-cms.omdat.id/storage/pois/60ba450fb296fa521956bd15/80b5d02e73436cd1645d7f8781730bc9.png",
                //                     15f,
                //                     histocache.image_aspect_ratio,
                //                     histocachingSpot.transform
                //                 );

                //                 m_POITitle.text = m_LanguageToggle.isOn ? histocache.title_en : histocache.title_de;

                //                 m_POIButton.onClick.RemoveAllListeners();
                //                 m_POIButton.onClick.AddListener(() => OnPOI(index));

                //                 m_POIButton.gameObject.SetActive(true);
                //             }
                //         });
                //     }
                //     else
                //     {
                //         m_HistocachePhoto.GetComponent<HistocachePhoto>().SetPhotoURL(
                //             "https://hcii-cms.omdat.id/storage/pois/60ba450fb296fa521956bd15/80b5d02e73436cd1645d7f8781730bc9.png",
                //             15f,
                //             histocache.image_aspect_ratio,
                //             histocachingSpot.transform
                //         );

                //         m_POITitle.text = m_LanguageToggle.isOn ? histocache.title_en : histocache.title_de;

                //         m_POIButton.onClick.RemoveAllListeners();
                //         m_POIButton.onClick.AddListener(() => OnPOI(index));

                //         m_POIButton.gameObject.SetActive(true);
                //     }
                // }

                // marker.GetComponent<Marker>().distanceLabel.text = (offset.x) + " | " + (offset.y);
            }
            // else
            // {
            //     if (marker.activeSelf)
            //         marker.SetActive(false);
            // }
        }

        private void GetHistocacheCollection()
        {
            // m_IsLoadingPOI = true;

            foreach (Transform child in transform)
                if (!( child.name == "Compass" || child.name == "Cube"))
                    GameObject.Destroy(child.gameObject);

            // for (int i = 0; i < markers.Count; ++i)
            // {
            //     GameObject gameObject = markers[i];
            //     gameObject.Destroy();
            // }

            histocacheCollection.Clear();

            histocacheMarkers.Clear();
            viewpointMarkers.Clear();
            photos.Clear();

            // this.poiCollection.Clear();

            // if (data.histocacheCollection.Count == 0)
            //     data.FetchPoiCollection();

            DataManager.Instance.GetHistocacheCollection((Histocache[] histocacheCollection) =>
            {
                foreach (Histocache histocache in histocacheCollection)
                {
                    this.histocacheCollection[histocache._id] = histocache;

                    SetMarker(histocache);
                }
            });
        }

        // public void GetPOIDocument(Action<POI> callback, string poiId)
        // {
        //     if (m_IsLoadingPOIDocument)
        //         return;

        //     m_IsLoadingPOIDocument = true;

        //     // m_DebugText2.text += "GetPOIDocument begin\n";

        //     StartCoroutine(NetworkManager.GetPOIDocument((POI poi) =>
        //     {
        //         m_IsLoadingPOIDocument = false;

        //         callback(poi);

        //         // m_DebugText2.text += "GetPOIDocument end (" + poi?.image_url + ")\n";

        //     }, poiId));
        // }

        private void GetHistocache(string id, Action<Histocache> callback)
        {
            // if (m_IsLoadingPOIDocument)
                // return;

            // m_IsLoadingPOIDocument = true;

            DataManager.Instance.GetHistocache(id, (Histocache histocache) =>
            {
                // m_IsLoadingPOIDocument = false;

                callback(histocache);
            });
        }

        void OnPOI(string histocacheId)
        {
            documents.Show(m_LanguageToggle.isOn ? 1 : 0, histocacheCollection[histocacheId]);
        }

        public void SetLatestTargetRotation(Quaternion targetRotation)
        {
            m_LatestTargetRotation = targetRotation;
        }
    }
}
