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
        public Sprite[] ARImages;

        private static readonly string[,] ARStatuses = {{ "Initialisieren von Augmented Reality", "Initializing Augmented Reality" },
                                                        { "Konnte Augmented Reality nicht initialisieren", "Failed to initialize Augmented Reality" },
                                                        { "Zum Aussichtspunkt gehen Sie", "Walk to the viewpoint" },
                                                        { "Augmented Reality verlassen", "Leaving Augmented Reality" }};

        private static readonly string[] ARReturnTexts = { "Zurück zur Karte", "Return to Map"};

        private static readonly Color enabledColor = new Color(255/255f, 191/255f, 0/255f);
        private static readonly Color disabledColor = new Color(177/255f, 177/255f, 177/255f);

		public event Action<string> OnApproachingViewpoint = delegate { };
    	public event Action<string> OnLeavingViewpoint = delegate { };

        // AR Canvas
        public Canvas ARCanvas; 

        public Image ARCanvasImage;
        public Text ARCanvasText;
        public GameObject ARCanvasButton;
        public Text ARCanvasButtonText;

        private  float maxApproachingSqrDistance = 156.25f;
        private  float minLeavingSqrDistance = 400f;

        private  string closestId = null;

        public ARSession m_ARSession;

        private const float m_ARSessionTimeout = 10f;

        // View in AR
        public Button m_ViewInARButton;
        public Image m_ViewInARImage;
        public Text m_ViewInARText;

        private bool isTutorialShowing;
        private bool isLeaving;

        public ScreenManager screenManager;

        private Camera m_MainCamera;

        // private bool m_ARSupported;

        public GameObject locationTemplate;
        public GameObject viewpointTemplate;
        public GameObject photoTemplate;
        public GameObject photoTypeBTemplate;
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
        private float gpsLatitude = float.MinValue;
        private float gpsLongitude = float.MinValue;

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
            SM = StateManager.Instance;
            m_MainCamera = Camera.main;

            yield return null;
            _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
            _locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;

            yield return CheckARAvailability();
        }

		void LocationProvider_OnLocationUpdated(Mapbox.Unity.Location.Location location)
		{
            if (ARSession.state == ARSessionState.Unsupported)
            {
                _locationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
            }
            else if (ARSession.state == ARSessionState.Ready)
            {
                CheckApproaching((float) location.LatitudeLongitude.x, (float) location.LatitudeLongitude.y);
            }
            else if (ARSession.state == ARSessionState.SessionTracking)
            {
                if (isTutorialShowing || isLeaving)
                    return;

                CheckLeaving();
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
                m_ViewInARButton.gameObject.SetActive(true);
            }
            else
            {
                // Allow the AR session
                m_ViewInARButton.gameObject.SetActive(true);

                GetHistocacheCollection(() => {});
            }
        }

        private void CheckApproaching(float latitude, float longitude)
        {
            float closestSqrDistance = maxApproachingSqrDistance;

            foreach (Histocache histocache in histocacheCollection.Values)
            {
                if (!histocache.has_viewpoint_location || !histocache.has_histocache_location)
                    continue;

                Vector2 offset = HistocacheConversions.GeoToUnityPosition(histocache.viewpoint_lat, histocache.viewpoint_long, latitude, longitude);

                float sqrDistance = offset.sqrMagnitude;

                if (sqrDistance <= closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closestId = histocache._id;
                }
            }

            if (closestId != null)
            {
                OnApproachingViewpoint(closestId);
                m_ViewInARButton.interactable = true;
                m_ViewInARImage.color = enabledColor;
                m_ViewInARText.color = enabledColor;
            }
            else
            {
                m_ViewInARButton.interactable = false;
                m_ViewInARImage.color = disabledColor;
                m_ViewInARText.color = disabledColor;
            }
        }

        private void CheckLeaving()
        {
            // if (closestId != null && histocacheCollection.TryGetValue(closestId, out Histocache histocache))
            // {
            //     Vector2 offset = HistocacheConversions.GeoToUnityPosition(histocache.viewpoint_lat, histocache.viewpoint_long, latitude, longitude);

            //     float sqrDistance = offset.sqrMagnitude;

            //     if (sqrDistance > minLeavingSqrDistance)
            //     {
            //         isLeaving = true;

            //         OnLeavingViewpoint(closestId);

            //         StartCoroutine(Leaving());
            //     }
            // }

            if (m_Viewpoint != null)
            {
                Vector3 diff = m_Viewpoint.transform.position - m_MainCamera.transform.position;
                diff.y = 0;

                float sqrDistance = diff.sqrMagnitude;

                Debug.Log("SqrDistance " + sqrDistance);

                if (sqrDistance > minLeavingSqrDistance)
                {
                    isLeaving = true;

                    OnLeavingViewpoint(closestId);

                    StartCoroutine(Leaving());
                }
            }
        }

        private IEnumerator Leaving()
        {
            ARCanvasImage.sprite = ARImages[3];
            ARCanvasText.text = ARStatuses[3, m_LanguageToggle.isOn ? 0 : 1];
            // ARCanvasButtonText.text = ARReturnTexts[m_LanguageToggle.isOn ? 0 : 1];
            // ARCanvasButton.SetActive(true);
            ARCanvasButton.SetActive(false);

            ARCanvas.gameObject.SetActive(true);

            float time = 0;

            while (time < 5f)
            {
                yield return null;
                time += Time.deltaTime;
            }

            ARCanvas.gameObject.SetActive(false);

            screenManager.SwitchToMapScreen();
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

        public IEnumerator GenerateWorld(string id, Action<bool> callback)
        {
            // Vector3 targetPosition = m_MainCamera.transform.position;
			// targetPosition.y -= 1.8f;
			// transform.position = targetPosition;

            isTutorialShowing = false;
            isLeaving = false;

            ARCanvasImage.sprite = ARImages[0];
            ARCanvasText.text = ARStatuses[0, m_LanguageToggle.isOn ? 0 : 1];
            ARCanvasButton.SetActive(false);

            ARCanvas.gameObject.SetActive(true);

            yield return null;

            m_ARSession.enabled = true;

            m_ARSession.Reset();

            float time = 0;

            while (time < m_ARSessionTimeout && ARSession.state < ARSessionState.SessionTracking)
            {
                yield return null;
                time += Time.deltaTime;
            }

            if (ARSession.state < ARSessionState.SessionTracking)
            {
                // SM.SetState(State.Map);

                ARCanvasImage.sprite = ARImages[1];
                ARCanvasText.text = ARStatuses[1, m_LanguageToggle.isOn ? 0 : 1];
                // ARCanvasButtonText.text = ARReturnTexts[m_LanguageToggle.isOn ? 0 : 1];
                // ARCanvasButton.SetActive(true);
                ARCanvasButton.SetActive(false);

                ARCanvas.gameObject.SetActive(true);

                callback(false);

                time = 0;

                while (time < 5f)
                {
                    yield return null;
                    time += Time.deltaTime;
                }

                ARCanvas.gameObject.SetActive(false);

                yield break;
            }

            isTutorialShowing = true;

            ARCanvasImage.sprite = ARImages[2];
            ARCanvasText.text = ARStatuses[2, m_LanguageToggle.isOn ? 0 : 1];
            ARCanvasButton.SetActive(false);

            ARCanvas.gameObject.SetActive(true);

            transform.localRotation = m_LatestTargetRotation;
            gpsLatitude = (float) LocationProvider.CurrentLocation.LatitudeLongitude.x;
            gpsLongitude = (float) LocationProvider.CurrentLocation.LatitudeLongitude.y;

            if (string.IsNullOrWhiteSpace(id))
            {
                GetHistocacheCollection(() => SetMarkers());
            }
            else
            {
                documents.gameObject.SetActive(false);
                GameObject.Find("PlacesCancvas").SetActive(false);

                GameObject go = Instantiate(photoTypeBTemplate, transform, false);
                GetHistocache(id, (Histocache histocache) =>
                {
                    go.GetComponent<HistocachePhoto>().SetPhotoURL(
                        histocache.image_url,
                        1f,
                        histocache.image_aspect_ratio,
                        1f
                    );
                });

                SetDetail(id);
            }

            callback(true);

            time = 0;

            while (time < 5f)
            {
                yield return null;
                time += Time.deltaTime;
            }

            ARCanvas.gameObject.SetActive(false);

            isTutorialShowing = false;
        }

        public void DestroyWorld()
        {
            ARCanvas.gameObject.SetActive(false);

            m_ARSession.enabled = false;

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

            if (m_RotationPivot != null)
            {
                GameObject.Destroy(m_RotationPivot);
                m_RotationPivot = null;
            }

            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }

        GameObject GetHistocacheMarker(string histocacheId)
        {
            GameObject marker;

            if (!markers.TryGetValue(histocacheId, out marker))
            {
                marker = Instantiate(locationTemplate, transform, false);

                markers.Add(histocacheId, marker);
            }

            return marker;
        }

        private void SetMarkers()
        {
            foreach (Histocache h in histocacheCollection.Values)
            {
                if (!h.has_histocache_location)
                    continue;
                
                // Histocache marker
                Vector2 offset = HistocacheConversions.GeoToUnityPosition(h.lat, h.@long, gpsLatitude, gpsLongitude);

                if (offset.x > m_MainCamera.farClipPlane)
                    continue;

                GameObject marker = GetHistocacheMarker(h._id);
                marker.transform.localPosition = new Vector3(offset.y, 0, offset.x);

                // Rescale
                float scale = 1f + (Mathf.Max(offset.x, offset.y) / 50f);
                marker.transform.localScale = new Vector3(scale, scale, scale);

                marker.SetActive(true);
            }

            if (closestId == null)
                return;

            GameObject closestMarker = GetHistocacheMarker(closestId);
            closestMarker.transform.GetChild(0).gameObject.SetActive(false);

            Histocache histocache = histocacheCollection[closestId];

            // Viewpoint
            Vector2 viewpointOffset = HistocacheConversions.GeoToUnityPosition(histocache.viewpoint_lat, histocache.viewpoint_long, gpsLatitude, gpsLongitude);

            if (m_Viewpoint == null)
                m_Viewpoint = Instantiate(viewpointTemplate, transform, false);

            m_Viewpoint.transform.localPosition = new Vector3(viewpointOffset.y, 0, viewpointOffset.x);
            m_Viewpoint.transform.LookAt(closestMarker.transform.position);

            // Rotation pivot
            if (m_RotationPivot == null)
                m_RotationPivot = new GameObject("RotationPivot");

            m_RotationPivot.transform.position = m_Viewpoint.transform.position;
            transform.SetParent(m_RotationPivot.transform);

            // Histocache line
            if (m_HistocacheLine == null)
                m_HistocacheLine = Instantiate(lineTemplate, transform, false);
            
            var points = new Vector3[2] { m_Viewpoint.transform.localPosition, closestMarker.transform.localPosition};
            m_HistocacheLine.GetComponent<HistocacheLine>().SetPositions(points);

            // Histocache photo
            if (m_HistocachePhoto == null)
                m_HistocachePhoto = Instantiate(photoTemplate, transform, false);

            m_HistocachePhoto.transform.localPosition = closestMarker.transform.localPosition;
            m_HistocachePhoto.transform.LookAt(m_Viewpoint.transform.position);

            GetHistocache(histocache._id, (Histocache histocache) =>
            {
                m_HistocachePhoto.GetComponent<HistocachePhoto>().SetPhotoURL(
                    histocache.viewpoint_image_url,
                    histocache.viewpoint_image_height,
                    histocache.viewpoint_image_aspect_ratio,
                    histocache.viewpoint_image_offset
                );

                SetDetailTitle(m_LanguageToggle.isOn ? histocache.title_en : histocache.title_de);

                m_DetailBtn.onClick.RemoveAllListeners();
                m_DetailBtn.onClick.AddListener(() => OnHistocache(histocache._id));

                m_DetailBtn.gameObject.SetActive(true);
            });
        }

        private void SetDetail(string id)
        {
            GetHistocache(id, (Histocache histocache) =>
            {
                SetDetailTitle(m_LanguageToggle.isOn ? histocache.title_en : histocache.title_de);

                m_DetailBtn.onClick.RemoveAllListeners();
                m_DetailBtn.onClick.AddListener(() => OnHistocache(histocache._id));

                m_DetailBtn.gameObject.SetActive(true);
            });
        }

        private void GetHistocacheCollection(Action callback)
        {
            if (histocacheCollection.Count == 0)
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
            else
            {
                callback();
            }
        }

        private void GetHistocache(string id, Action<Histocache> callback)
        {
			if (histocacheCollection.TryGetValue(id, out Histocache histocache))
			{
				if (string.IsNullOrWhiteSpace(histocache.viewpoint_image_url))			
				{
					Debug.Log("World::GetHistocache " + id);

					DataManager.Instance.GetHistocache(id, (Histocache h) =>
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

							histocache.has_histocache_location = h.has_histocache_location;
							histocache.has_viewpoint_location = h.has_viewpoint_location;

							histocache.add_info_url = h.add_info_url;

							histocache.documents = h.documents;

							callback(histocache);
						}
					});
				}
				else
				{
					callback(histocache);
				}
			}
        }

        private void SetDetailTitle(string title)
        {
            string[] texts = title.Split('(');

            m_DetailBtnLabel.text = texts[0];
        }

        private void OnHistocache(string histocacheId)
        {
            documents.Show(m_LanguageToggle.isOn ? 1 : 0, histocacheCollection[histocacheId]);
        }

        public void SetLatestTargetRotation(Quaternion targetRotation)
        {
            m_LatestTargetRotation = targetRotation;
        }
    }
}
