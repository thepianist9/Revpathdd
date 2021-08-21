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
    public class OldWorld : MonoBehaviour
    {
        public TMP_Text m_DebugText1;
        public TMP_Text m_DebugText2;

        private List<POI> poiCollection = new List<POI>();

        private Camera m_MainCamera;

        public GameObject markerTemplate;
        public GameObject photoTemplate;

        private List<GameObject> markers = new List<GameObject>();

        private GameObject m_HistocachePhoto = null;

        private bool m_IsLoadingPOI = false;
        private bool m_IsLoadingPOIDocument = false;

        public Button m_POIButton;
        public Text m_POITitle;

        public Toggle m_LanguageToggle;

        public Documents poiDetail;

        // Location
        private double gpsLatitude = float.MinValue;
        private double gpsLongitude = float.MinValue;

        private int m_LocationUpdatedCounter;
        private bool m_IsFirstRotation = true;
        private float m_PreviousRotationAngle;
        private float m_RotationAngleThreshold = 30f;
        private int m_CurrentPassThresholdRotations = 0;
        private int m_MaxPassThresholdRotations = 5;

        /// <summary>
        /// The rate at which the transform's rotation tries catch up to the provided heading.  
        /// </summary>
        [SerializeField]
        [Tooltip("The rate at which the transform's rotation tries catch up to the provided heading. ")]
        float _rotationFollowFactor = 1;

        /// <summary>
        /// <para>Set this to true if you'd like to adjust the sign of the rotation angle.</para>
        /// <para>eg angle passed in 63.5, angle that should be used for rotation: -63.5.</para>
        /// <para>This might be needed when rotating the map and not objects on the map.</para>
        /// </summary>
        [SerializeField]
        [Tooltip("Set this to true if you'd like to adjust the sign of the rotation angle. eg angle passed in 63.5, angle that should be used for rotation: -63.5.")]
        bool _useNegativeAngle;

        Quaternion _targetRotation;

        float _targetRotationDegree;

        /// <summary>
        /// The location provider.
        /// This is public so you change which concrete <see cref="ILocationProvider"/> to use at runtime.  
        /// </summary>
        ILocationProvider _locationProvider;
        public ILocationProvider LocationProvider
        {
            private get
            {
                if (_locationProvider == null)
                {
                    _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
                }
                return _locationProvider;
            }
            set
            {
                if (_locationProvider != null)
                {
                    _locationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
                }
                _locationProvider = value;
                _locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
            }
        }

        Vector3 _targetPosition;

        void Start()
        {
            m_MainCamera = Camera.main;

            LocationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
        }

        void OnDestroy()
        {
            if (LocationProvider != null)
            {
                LocationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
            }
        }

        void LocationProvider_OnLocationUpdated(Location location)
        {
            // m_DebugText1.text = Time.deltaTime + " | " + location.LatitudeLongitude.x + " | " + location.LatitudeLongitude.y;

            gpsLatitude = location.LatitudeLongitude.x;
            gpsLongitude = location.LatitudeLongitude.y;

            // if (!m_IsLoadingPOI)
            // {
            //     m_IsLoadingPOI = true;
            GetPOICollection();
            // }

            float rotationAngle = location.DeviceOrientation;

			if (_useNegativeAngle) { rotationAngle *= -1f; }

            rotationAngle += m_MainCamera.transform.localEulerAngles.y;
            if (rotationAngle < 0) { rotationAngle += 360; }
            if (rotationAngle >= 360) { rotationAngle -= 360; }

            // m_DebugText1.text = "OnLocationUpdated: " + ++m_LocationUpdatedCounter + "\n"
			// 	+ "gpsLatitude: " + gpsLatitude + "\n"
            //     + "gpsLongitude: " + gpsLongitude + "\n"
			// 	+ "rotationAngle: " + rotationAngle + "\n"
			// 	+ "m_CurrentPassThresholdRotations: " + m_CurrentPassThresholdRotations + "\n";

            if (!m_IsFirstRotation)
                // Handle rotation changes with threshold
                if (Mathf.Abs(m_PreviousRotationAngle - rotationAngle) > m_RotationAngleThreshold)
                {
                    ++m_CurrentPassThresholdRotations;

                    if (m_CurrentPassThresholdRotations > m_MaxPassThresholdRotations)
                    {
                        m_PreviousRotationAngle = rotationAngle;
                        m_CurrentPassThresholdRotations = 0;
                    }
                    else
                    {
                        rotationAngle = m_PreviousRotationAngle;
                    }
                }
                else
                {
                    rotationAngle = m_PreviousRotationAngle;
                    m_CurrentPassThresholdRotations = 0;
                }
            else
                m_IsFirstRotation = false;

            _targetRotation = Quaternion.Euler(getNewEulerAngles(rotationAngle));
        }

		private Vector3 getNewEulerAngles(float newAngle)
		{
			var localRotation = transform.localRotation;
			var currentEuler = localRotation.eulerAngles;
			var euler = Mapbox.Unity.Constants.Math.Vector3Zero;

			euler.y = newAngle;
			euler.x = currentEuler.x;
			euler.z = currentEuler.z;

			return euler;
		}

        void Update()
        {
            Vector3 targetPosition = m_MainCamera.transform.position;
			targetPosition.y -= 1.8f;
			transform.position = targetPosition;

            transform.localRotation = Quaternion.Lerp(transform.localRotation, _targetRotation, Time.deltaTime * _rotationFollowFactor);

            // TODO: move this somewhere else and only update every 0.5 second
            int count = 0;
            GameObject m = null;
            int index = 0;

            for (int i = 0; i < markers.Count; ++i)
            {
                GameObject gameObject = markers[i];
                if (!gameObject.activeSelf)
                    continue;

                Vector3 target = gameObject.transform.position - transform.position;
                float angle = Vector3.Angle(target, m_MainCamera.transform.forward);
                target.y = 0;

                // m_DebugText1.text = "sqrMagnitude " + target.sqrMagnitude;

                // TODO: find real FOV calculation & do not hardcode the squared distance
                if (angle <= 30 && target.sqrMagnitude <= 160000)
                {
                    count += 1;
                    m = gameObject;
                    index = i;

                    // text += i + " " + gameObject.transform.position + " " + gameObject.transform.localPosition + "\n";
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

                    // Vector3 forward = m_MainCamera.transform.position - m.transform.position;
                    // m_HistocachePhoto.transform.Translate(forward * 0.1f);

                    POI poi = poiCollection[index];

                    m_POITitle.text = m_LanguageToggle.isOn ? poi.title_en : poi.title_de;
                    
                    m_POIButton.onClick.RemoveAllListeners();
                    m_POIButton.onClick.AddListener(() => OnPOI(index));

                    m_POIButton.gameObject.SetActive(true);

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

                                m_HistocachePhoto.GetComponent<HistocachePhoto>().SetPhotoURL(poi.image_url, poi.image_aspect_ratio, transform);
                            }

                        }, poi.id);
                    }
                    else
                    {
                        m_HistocachePhoto.GetComponent<HistocachePhoto>().SetPhotoURL(poi.image_url, poi.image_aspect_ratio, transform);
                    }
                }
            }
            else
            {
                if (m_HistocachePhoto)
                    m_HistocachePhoto.SetActive(false);

                m_POIButton.gameObject.SetActive(false);
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

            GameObject marker = GetMarker(index);

            Vector2 offset = Conversions.GeoToUnityPosition(poi.lat, poi.@long, (float) gpsLatitude, (float) gpsLongitude);
            if (offset.x < m_MainCamera.farClipPlane)
            {
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
            }
            else
            {
                if (marker.activeSelf)
                    marker.SetActive(false);
            }
        }

        public void GetPOICollection()
        {
            if (m_IsLoadingPOI)
            {
                for (int i = 0; i < this.poiCollection?.Count; ++i)
                {
                    POI poi = this.poiCollection[i];

                    SetMarker(i);
                }

                return;
            }

            m_IsLoadingPOI = true;

            // m_DebugText2.text += "GetPOICollection begin\n";

            this.poiCollection.Clear();

            StartCoroutine(NetworkManager.GetPOICollection((POI[] poiCollection) =>
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

            StartCoroutine(NetworkManager.GetPOIDocument((POI poi) =>
            {
                m_IsLoadingPOIDocument = false;

                callback(poi);

                // m_DebugText2.text += "GetPOIDocument end (" + poi?.image_url + ")\n";

            }, poiId));
        }

        void OnPOI(int index)
        {
        //     poiDetail.Show(m_LanguageToggle.isOn ? 1 : 0, poiCollection[index]);
        }
    }
}
