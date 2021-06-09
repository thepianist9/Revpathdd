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
    public class HeadingChangedEvent : UnityEvent<float>
    {

    }

    public class LocationService : MonoBehaviour
    {
        public const float UPDATE_TIME = 1f;

		[SerializeField]
		float desiredAccuracyInMeters = 5f;

		[SerializeField]
		float updateDistanceInMeters = 5f;

        public GameObject m_gpsUIText;

        public LocationChangedEvent locationChangedEvent;
        public HeadingChangedEvent headingChangedEvent;

        private IEnumerator coroutine;

        // Location
        private float altitude = float.MinValue;
        private float latitude = float.MinValue;
        private float longitude = float.MinValue;

        private float horizontalAccuracy = float.MinValue;
        private float verticalAccuracy = float.MinValue;

        private double timestamp = float.MinValue;

        // Heading
        private float trueHeading = float.MinValue;

        //private KalmanCompass kalmanCompass;

        void Awake()
        {
            #if UNITY_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
                    Permission.RequestUserPermission(Permission.FineLocation);
                }
            #endif

            // kalmanCompass = new KalmanCompass(1f);
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
            Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);

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

            float t = 0;

            while (true)
            {
                t += Time.deltaTime;

//                if (t >= UPDATE_TIME)
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

                        if (locationChanged)
                            locationChangedEvent?.Invoke(altitude, latitude, longitude, timestamp);

                        // string info = "Latitude: " + latitude + "\n" +
                        //             "Longitude: " + longitude + "\n" +
                        //             "Altitude: " + altitude + "\n" +
                        //             "Hor Accuracy: " + horizontalAccuracy + "\n" +
                        //             "Timestamp: " + timestamp;

                        // m_gpsUIText.GetComponent<TMP_Text>().text = info;
                    }
                }

                // // Compass
                float heading = Input.compass.trueHeading;

                //kalmanCompass.Process(heading, Input.compass.headingAccuracy, Input.compass.timestamp);

//                if (t >= UPDATE_TIME)
                {
                    bool headingChanged = true;//!Math.Approximately(trueHeading, kalmanCompass.Heading);

                    trueHeading = heading;

                    if (headingChanged)
                        headingChangedEvent?.Invoke(trueHeading);
                }

                if (t >= UPDATE_TIME)
                    t = 0;

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
