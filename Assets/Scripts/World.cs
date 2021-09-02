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
		public event Action OnReady = delegate { };

        private Camera m_MainCamera;

        private bool m_ARSupported;
        private bool m_LocationAvailable;

        public GameObject locationTemplate;
        public GameObject viewpointTemplate;
        public GameObject photoTemplate;
        public GameObject lineTemplate;

        private Dictionary<string, Histocache> histocacheCollection = new Dictionary<string, Histocache>();

        private Dictionary<string, GameObject> markers = new Dictionary<string, GameObject>();

        private GameObject m_Viewpoint = null;
        private GameObject m_HistocacheLine = null;
        private GameObject m_HistocachePhoto = null;

        public Button m_DetailBtn;
        public Text m_DetailBtnLabel;

        public Toggle m_LanguageToggle;

        public Documents documents;

        // Location
        private double gpsLatitude = float.MinValue;
        private double gpsLongitude = float.MinValue;

        Quaternion m_LatestTargetRotation;

        private StateManager SM;

        private bool m_IsTouchReset = true;

        private Vector3 m_OriginalTransformPos;
        private Vector3 m_TouchStartPos;
        private Vector2 m_TouchOffsetPos;
        private Vector2 m_RotateDirectionStart;

        private GameObject m_RotationPivot;

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

        protected virtual IEnumerator Start()
        {
            yield return null;
            _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
            _locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;

            ARSession.stateChanged += OnStateChanged;

            SM = StateManager.Instance;
            m_MainCamera = Camera.main;

            yield return CheckARAvailability();
        }

		void LocationProvider_OnLocationUpdated(Mapbox.Unity.Location.Location location)
		{
            if (ARSession.state == ARSessionState.Unsupported)
            {
                _locationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
            }
            else if (ARSession.state == ARSessionState.None || ARSession.state == ARSessionState.CheckingAvailability || ARSession.state == ARSessionState.None)
            {

            }
            else if (ARSession.state == ARSessionState.SessionTracking)
            {
                // TODO check distance
            }

			// GetHistocacheCollection(() => SetMarkers());
		}

        private void OnStateChanged(ARSessionStateChangedEventArgs args)
        {
            switch(args.state)
            {
                case ARSessionState.None:
                    break;
                case ARSessionState.CheckingAvailability:
                    break;
                case ARSessionState.Installing:
                    break;
                case ARSessionState.NeedsInstall:
                    break;
                case ARSessionState.Ready:

                    break;
                case ARSessionState.SessionInitializing:
                    break;
                case ARSessionState.SessionTracking:
                    break;
                case ARSessionState.Unsupported:
                    break;
            }
        }

        private IEnumerator CheckARAvailability()
        {
            if (ARSession.state == ARSessionState.None || ARSession.state == ARSessionState.CheckingAvailability)
            {
                yield return ARSession.CheckAvailability();
            }

            if (ARSession.state == ARSessionState.Unsupported)
            {
                // Start some fallback experience for unsupported devices
                m_ARSupported = false;
            }
            else
            {
                // Allow the AR session
                m_ARSupported = true;
            }    
        }

        void Update()
        {
            // GameObject.Find("DebugText1").GetComponent<TMP_Text>().text = "DeviceOrientation = " + Input.gyro.attitude.eulerAngles.x;
            // GameObject.Find("DebugText1").GetComponent<TMP_Text>().text = "++\n";
            // foreach (ARAnchor anchor in m_ARAnchorManager.trackables)
            // {
            //     GameObject.Find("DebugText1").GetComponent<TMP_Text>().text += anchor.name + "\n";
            // }

            if (SM.state == State.Camera)
            {
                if (Input.touchCount == 0)
                {
                    m_IsTouchReset = true;
                }
                // World pan
                else if (Input.touchCount == 1)
                {
                    if (m_IsTouchReset)
                    {
                        if (Input.GetTouch(0).phase == TouchPhase.Began)
                        {
                            m_TouchStartPos = m_MainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_MainCamera.nearClipPlane));
                            m_OriginalTransformPos = m_RotationPivot.transform.position;
                        }
                        else if (Input.GetTouch(0).phase == TouchPhase.Moved)
                        {
                            Vector3 currentTouchPos = m_MainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_MainCamera.nearClipPlane));
                            m_TouchOffsetPos = new Vector2(currentTouchPos.x - m_TouchStartPos.x, currentTouchPos.z - m_TouchStartPos.z);
                            m_RotationPivot.transform.position = m_OriginalTransformPos + new Vector3(m_TouchOffsetPos.x * 20, 0, m_TouchOffsetPos.y * 20);
                        }
                    }
                }
                // World rotation
                else if (Input.touchCount == 2)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began)
                    {
                        m_RotateDirectionStart = Input.GetTouch(1).position - Input.GetTouch(0).position;
                        m_IsTouchReset = false;
                    }
                    else if (Input.GetTouch(0).phase == TouchPhase.Stationary && Input.GetTouch(1).phase == TouchPhase.Stationary)
                    {
                        m_RotateDirectionStart = Input.GetTouch(1).position - Input.GetTouch(0).position;
                    }
                    else if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
                    {
                        Vector2 CurrentRotateDirection = Input.GetTouch(1).position - Input.GetTouch(0).position;
                        var angle = Vector2.SignedAngle(CurrentRotateDirection, m_RotateDirectionStart) * 0.01f;

                        m_RotationPivot.transform.Rotate(0, angle, 0);
                    }
                }
            }

            // GameObject.Find("DebugText1").GetComponent<TMP_Text>().text = point.ToString("F3");
        }

        public void GenerateWorld()
        {
            // Vector3 targetPosition = m_MainCamera.transform.position;
			// targetPosition.y -= 1.8f;
			// transform.position = targetPosition;

            transform.localRotation = m_LatestTargetRotation;
            gpsLatitude = LocationProvider.CurrentLocation.LatitudeLongitude.x;
            gpsLongitude = LocationProvider.CurrentLocation.LatitudeLongitude.y;

            GetHistocacheCollection(() => SetMarkers());
        }

        public void DestroyWorld()
        {
            histocacheCollection.Clear();

            foreach (GameObject marker in markers.Values)
            {
                GameObject.Destroy(marker);
            }

            markers.Clear();

            if (m_Viewpoint != null)
            {
                GameObject.Destroy(m_Viewpoint);
                m_Viewpoint = null;
            }

            if (m_HistocacheLine != null)
            {
                GameObject.Destroy(m_HistocacheLine);
                m_HistocacheLine = null;
            }

            if (m_HistocachePhoto != null)
            {
                GameObject.Destroy(m_HistocachePhoto);
                m_HistocachePhoto = null;
            }

            transform.SetParent(null);
            GameObject.Destroy(m_RotationPivot);

            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }

        GameObject GetHistocacheMarker(string histocacheId)
        {
            GameObject marker;

            if (!markers.TryGetValue(histocacheId, out marker))
            {
                marker = Instantiate(locationTemplate, transform, false);
                //marker.GetComponent<POIBillboard>().POIClickedEvent.AddListener(OnPoiClicked);

                markers.Add(histocacheId, marker);
            }

            return marker;
        }

        // GameObject GetViewpointMarker(string histocacheId)
        // {
        //     GameObject marker;

        //     if (!viewpointMarkers.TryGetValue(histocacheId, out marker))
        //     {
        //         marker = Instantiate(viewpointTemplate, transform, false);
        //         //marker.GetComponent<POIBillboard>().POIClickedEvent.AddListener(OnPoiClicked);

        //         viewpointMarkers.Add(histocacheId, marker);
        //     }

        //     return marker;
        // }

        // HistocachePhoto GetPhoto(string histocacheId)
        // {
        //     GameObject photo;

        //     if (!photos.TryGetValue(histocacheId, out photo))
        //     {
        //         photo = Instantiate(photoTemplate, transform, false);

        //         photos.Add(histocacheId, photo);
        //     }

        //     return photo.GetComponent<HistocachePhoto>();
        // }

        private void SetMarkers()
        {
            string closestId = null;

            float closestDistance = float.MaxValue;

            foreach (Histocache h in histocacheCollection.Values)
            {
                if (!h.has_histocache_location)
                    continue;
                
                // Histocache marker
                Vector2 offset = Conversions.GeoToUnityPosition(h.lat, h.@long, (float) gpsLatitude, (float) gpsLongitude);
                // if (offset.x < m_MainCamera.farClipPlane)
                {
                    GameObject m = GetHistocacheMarker(h._id);
                    m.transform.localPosition = new Vector3(offset.y, 0, offset.x);

                    // Rescale
                    float scale = 1f + (Mathf.Max(offset.x, offset.y) / 50f);
                    m.transform.localScale = new Vector3(scale, scale, scale);

                    if (!m.activeSelf)
                        m.SetActive(true);

                    if (!h.has_viewpoint_location)
                        continue;

                    offset = Conversions.GeoToUnityPosition(h.viewpoint_lat, h.viewpoint_long, (float) gpsLatitude, (float) gpsLongitude);

                    if (offset.sqrMagnitude < closestDistance)
                    {
                        closestDistance = offset.sqrMagnitude;
                        closestId = h._id;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(closestId))
                return;

            GameObject closestMarker = GetHistocacheMarker(closestId);
            closestMarker.transform.GetChild(0).gameObject.SetActive(false);

            Histocache histocache = histocacheCollection[closestId];

            // Viewpoint
            Vector2 viewpointOffset = Conversions.GeoToUnityPosition(histocache.viewpoint_lat, histocache.viewpoint_long, (float) gpsLatitude, (float) gpsLongitude);

            if (m_Viewpoint == null)
                m_Viewpoint = Instantiate(viewpointTemplate, transform, false);

            m_Viewpoint.transform.localPosition = new Vector3(viewpointOffset.y, 0, viewpointOffset.x);

            GameObject marker = markers[histocache._id];
            m_Viewpoint.transform.LookAt(marker.transform.position);

            m_RotationPivot = new GameObject("RotationPivot");
            m_RotationPivot.transform.position = m_Viewpoint.transform.position;
            transform.SetParent(m_RotationPivot.transform);

            // Histocache line
            if (m_HistocacheLine == null)
                m_HistocacheLine = Instantiate(lineTemplate, transform, false);
            
            var points = new Vector3[2] { m_Viewpoint.transform.localPosition, marker.transform.localPosition};
            m_HistocacheLine.GetComponent<HistocacheLine>().SetPositions(points);

            // Histocache Photo
            if (m_HistocachePhoto == null)
                m_HistocachePhoto = Instantiate(photoTemplate, transform, false);

            m_HistocachePhoto.transform.localPosition = marker.transform.localPosition;
            m_HistocachePhoto.transform.LookAt(m_Viewpoint.transform.position);
            
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

                        m_HistocachePhoto.GetComponent<HistocachePhoto>().SetPhotoURL(
                            histocache.viewpoint_image_url,
                            histocache.viewpoint_image_height,
                            histocache.viewpoint_image_aspect_ratio,
                            histocache.viewpoint_image_offset
                        );

                        SetDetailTitle(m_LanguageToggle.isOn ? histocache.title_en : histocache.title_de);

                        m_DetailBtn.onClick.RemoveAllListeners();
                        m_DetailBtn.onClick.AddListener(() => OnPOI(histocache._id));

                        m_DetailBtn.gameObject.SetActive(true);
                    }
                });
            }
            else
            {
                m_HistocachePhoto.GetComponent<HistocachePhoto>().SetPhotoURL(
                    histocache.viewpoint_image_url,
                    histocache.viewpoint_image_height,
                    histocache.viewpoint_image_aspect_ratio,
                    histocache.viewpoint_image_offset
                );

                SetDetailTitle(m_LanguageToggle.isOn ? histocache.title_en : histocache.title_de);

                m_DetailBtn.onClick.RemoveAllListeners();
                m_DetailBtn.onClick.AddListener(() => OnPOI(histocache._id));

                m_DetailBtn.gameObject.SetActive(true);
            }
        }

        private void GetHistocacheCollection(Action callback)
        {
            DataManager.Instance.GetHistocacheCollection((Histocache[] histocacheCollection) =>
            {
                foreach (Histocache histocache in histocacheCollection)
                {
                    this.histocacheCollection[histocache._id] = histocache;
                }

                callback();
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

        private void SetDetailTitle(string title)
        {
            string[] texts = title.Split('(');

            if (texts.Length >= 0)
            {
                m_DetailBtnLabel.text = texts[0];
            }
            else
            {
                m_DetailBtnLabel.text = "";
            }
        }

        private void OnPOI(string histocacheId)
        {
            documents.Show(m_LanguageToggle.isOn ? 1 : 0, histocacheCollection[histocacheId]);
        }

        public void SetLatestTargetRotation(Quaternion targetRotation)
        {
            m_LatestTargetRotation = targetRotation;
        }
    }
}
