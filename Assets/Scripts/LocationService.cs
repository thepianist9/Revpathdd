using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

namespace HistocachingII
{
    [System.Serializable]
    public class LocationChangedEvent : UnityEvent<float, float, float, double>
    {

    }

    [System.Serializable]
    public class CompassChangedEvent : UnityEvent<float>
    {

    }

    public class LocationService : MonoBehaviour
    {
        public const float UPDATE_TIME = 0.5f;

        public GameObject m_gpsUIText;

        public LocationChangedEvent locationChangedEvent;
        public CompassChangedEvent compassChangedEvent;

        private IEnumerator coroutine;

        // Location
        private float altitude = float.MinValue;
        private float latitude = float.MinValue;
        private float longitude = float.MinValue;

        private float horizontalAccuracy = float.MinValue;
        private float verticalAccuracy = float.MinValue;

        private double timestamp = float.MinValue;

        // Compass
        private float trueHeading = float.MinValue;

        private Transform m_mainCamera;

        void Awake()
        {
            #if UNITY_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
                    Permission.RequestUserPermission(Permission.FineLocation);
                }
            #endif

            m_mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
        }

        void Start()
        {
            coroutine = StartLocationService();

            StartCoroutine(coroutine);
        }

        void Destroy()
        {
            StopCoroutine(coroutine);

            StopLocationService();
        }

        IEnumerator StartLocationService()
        {
            // First, check if user has location service enabled
            if (!Input.location.isEnabledByUser)
                yield break;

            Input.compass.enabled = true;

            // Start service before querying location
            Input.location.Start(1);

            // Wait until service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1)
            {
                print("Timed out");
                yield break;
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                print("Unable to determine device location");
                yield break;
            }

            WaitForSeconds updateTime = new WaitForSeconds(UPDATE_TIME);

            while (true)
            {
                // Location
                if (Input.location.status == LocationServiceStatus.Running)
                {
                    LocationInfo lastData = Input.location.lastData;

                    bool locationChanged = !Mathf.Approximately(altitude, lastData.altitude) ||
                                        !Mathf.Approximately(latitude, lastData.latitude) ||
                                        !Mathf.Approximately(longitude, lastData.longitude);

                    altitude = lastData.altitude;
                    latitude = lastData.latitude;
                    longitude = lastData.longitude;

                    horizontalAccuracy = lastData.horizontalAccuracy;
                    verticalAccuracy = lastData.verticalAccuracy;

                    timestamp = lastData.timestamp;

                    // if (locationChanged)
                        locationChangedEvent?.Invoke(altitude, latitude, longitude, timestamp);

                    string info = "Latitude: " + latitude + "\n" +
                                "Longitude: " + longitude + "\n" +
                                "Altitude: " + altitude + "\n" +
                                "Hor Accuracy: " + horizontalAccuracy + "\n" +
                                "Timestamp: " + timestamp;

                    // m_gpsUIText.GetComponent<TMP_Text>().text = info;
                }

                // Compass
                Compass compass = Input.compass;

                bool compassChanged = !Mathf.Approximately(trueHeading, compass.trueHeading);

                trueHeading = compass.trueHeading;

                compassChangedEvent?.Invoke(trueHeading);

                yield return updateTime;
            }
        }

        void StopLocationService()
        {
            // Stop service if there is no need to query location updates continuously
            Input.location.Stop();
        }
    }
}
