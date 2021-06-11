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
        public TMP_Text _locationText;
        public TMP_Text _tmpText;

        private NetworkManager networkManager = new NetworkManager();

        private List<POI> poiCollection = new List<POI>();

        private Camera m_MainCamera;

        public GameObject markerTemplate;
        public GameObject photoTemplate;

        private List<GameObject> markers = new List<GameObject>();

        private GameObject m_POIPhoto = null;

        private bool m_IsLoadingPOI = false;
        private bool m_IsLoadingPOIDocument = false;

        public GameObject m_POIPanel;
        public Text m_POITitle;
        public Text m_POICaption;

        // Location
        private double gpsLatitude = float.MinValue;
        private double gpsLongitude = float.MinValue;

        /// <summary>
        /// Location property used for rotation: false=Heading (default), true=Orientation  
        /// </summary>
        [SerializeField]
        [Tooltip("Per default 'UserHeading' (direction the device is moving) is used for rotation. Check to use 'DeviceOrientation' (where the device is facing)")]
        bool _useDeviceOrientation;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        [Tooltip("Only evaluated when 'Use Device Orientation' is checked. Subtracts UserHeading from DeviceOrientation. Useful if map is rotated by UserHeading and DeviceOrientation should be displayed relative to the heading.")]
        bool _subtractUserHeading;

        /// <summary>
        /// The rate at which the transform's rotation tries catch up to the provided heading.  
        /// </summary>
        [SerializeField]
        [Tooltip("The rate at which the transform's rotation tries catch up to the provided heading. ")]
        float _rotationFollowFactor = 1;

        /// <summary>
        /// Set this to true if you'd like to adjust the rotation of a RectTransform (in a UI canvas) with the heading.
        /// </summary>
        [SerializeField]
        bool _rotateZ;

        /// <summary>
        /// <para>Set this to true if you'd like to adjust the sign of the rotation angle.</para>
        /// <para>eg angle passed in 63.5, angle that should be used for rotation: -63.5.</para>
        /// <para>This might be needed when rotating the map and not objects on the map.</para>
        /// </summary>
        [SerializeField]
        [Tooltip("Set this to true if you'd like to adjust the sign of the rotation angle. eg angle passed in 63.5, angle that should be used for rotation: -63.5.")]
        bool _useNegativeAngle;

        /// <summary>
        /// Use a mock <see cref="T:Mapbox.Unity.Location.TransformLocationProvider"/>,
        /// rather than a <see cref="T:Mapbox.Unity.Location.EditorLocationProvider"/>.   
        /// </summary>
        [SerializeField]
        bool _useTransformLocationProvider;

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
                    _locationProvider = _useTransformLocationProvider ?
                        LocationProviderFactory.Instance.TransformLocationProvider : LocationProviderFactory.Instance.DefaultLocationProvider;
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
            gpsLatitude = location.LatitudeLongitude.x;
            gpsLongitude = location.LatitudeLongitude.y;

            // if (!m_IsLoadingPOI)
            // {
            //     m_IsLoadingPOI = true;
            GetPOICollection();
            // }

            float rotationAngle = _useDeviceOrientation ? location.DeviceOrientation : location.UserHeading;

			if (_useNegativeAngle) { rotationAngle *= -1f; }

            // 'Orientation' changes all the time, pass through immediately
            if (_useDeviceOrientation)
            {
                if (_subtractUserHeading)
                {
                    if (rotationAngle > location.UserHeading)
                    {
                        rotationAngle = 360 - (rotationAngle - location.UserHeading);
                    }
                    else
                    {
                        rotationAngle = location.UserHeading - rotationAngle + 360;
                    }

                    if (rotationAngle < 0) { rotationAngle += 360; }
                    if (rotationAngle >= 360) { rotationAngle -= 360; }
                }

                rotationAngle += m_MainCamera.transform.localEulerAngles.y;
                if (rotationAngle < 0) { rotationAngle += 360; }
                if (rotationAngle >= 360) { rotationAngle -= 360; }

				_targetRotationDegree = rotationAngle;

                _targetRotation = Quaternion.Euler(getNewEulerAngles(rotationAngle));
            }
            else
            {
                // if rotating by 'Heading' only do it if heading has a new value
                if (location.IsUserHeadingUpdated)
                {
                    rotationAngle += m_MainCamera.transform.localEulerAngles.y;
                    if (rotationAngle < 0) { rotationAngle += 360; }
                    if (rotationAngle >= 360) { rotationAngle -= 360; }

					_targetRotation = Quaternion.Euler(getNewEulerAngles(rotationAngle));
                }
            }

            // _locationText.text = "Location: " + location.LatitudeLongitude + " | Rotation: " + rotationAngle + "\n"
            //     + "location.DeviceOrientation: " + location.DeviceOrientation + "\n"
            //     + "location.UserHeading: " + location.UserHeading + "\n"
            //     + "m_MainCamera.transform.localEulerAngles.y: " + m_MainCamera.transform.localEulerAngles.y; 
        }

        private Vector3 getNewEulerAngles(float newAngle)
        {
            var localRotation = transform.localRotation;
            var currentEuler = localRotation.eulerAngles;
            var euler = Mapbox.Unity.Constants.Math.Vector3Zero;

            euler.y = -newAngle;

            euler.x = 0;//currentEuler.x;
            euler.z = 0;//currentEuler.z;

            return euler;
        }

        void Update()
        {
            Vector3 targetPosition = m_MainCamera.transform.position;
			targetPosition.y -= 1.8f;
			transform.position = targetPosition;

            // transform.localRotation = Quaternion.Lerp(transform.localRotation, _targetRotation, Time.deltaTime * _rotationFollowFactor);
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, _targetRotationDegree, transform.localRotation.eulerAngles.z);

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
                    if (m_POIPhoto == null)
                        m_POIPhoto = Instantiate(photoTemplate, transform, false);

                    // Copying positions today does not work somehow.. it used to work
                    m_POIPhoto.transform.localPosition = new Vector3(m.transform.localPosition.x, 0, m.transform.localPosition.z);
                    // m_POIPhoto.transform.localPosition = new Vector3(-12.8f, 0, -168.6f);
                    m_POIPhoto.SetActive(true);

                    // _tmpText.text = "World::Update " + m.transform.localPosition;

                    // Vector3 forward = m_MainCamera.transform.position - m_POIPhoto.transform.position;
                    // m_POIPhoto.transform.Translate(forward * 0.1f);

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

                                m_POIPhoto.GetComponent<POIPhoto>().SetPhotoURL(poi.image_url);
                            }

                        }, poi.id);
                    }
                    else
                    {
                        m_POIPhoto.GetComponent<POIPhoto>().SetPhotoURL(poi.image_url);
                    }
                }
            }
            else
            {
                if (m_POIPhoto)
                    m_POIPhoto.SetActive(false);

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

            Vector2 offset = Conversions.GeoToUnityPosition(poi.lat, poi.@long, (float) gpsLatitude, (float) gpsLongitude);
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

        public void GetPOICollection()
        {
            if (m_IsLoadingPOI)
                return;

            m_IsLoadingPOI = true;

            _tmpText.text += "GetPOICollection begin\n";

            this.poiCollection.Clear();

            StartCoroutine(networkManager.GetPOICollection((POI[] poiCollection) =>
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

            StartCoroutine(networkManager.GetPOIDocument((POI poi) =>
            {
                m_IsLoadingPOIDocument = false;

                callback(poi);

                _tmpText.text += "GetPOIDocument end (" + poi?.image_url + ")\n";

            }, poiId));
        }
    }
}
