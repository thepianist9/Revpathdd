using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

namespace HistocachingII
{
    [System.Serializable]
    public class KompassChangedEvent : UnityEvent<float>
    {

    }

    public class CompassService : MonoBehaviour
    {
        public const float UPDATE_TIME = 1f;

		[SerializeField]
		float desiredAccuracyInMeters = 5f;

		[SerializeField]
		float updateDistanceInMeters = 5f;

        public GameObject m_gpsUIText;

        public KompassChangedEvent compassChangedEvent;

        private IEnumerator coroutine;

        // Compass
        private float trueHeading = float.MinValue;

        private KalmanCompass kalmanCompass;

        private Transform m_mainCamera;

        private float t = 0;

        void Awake()
        {
            #if UNITY_ANDROID
                if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
                    Permission.RequestUserPermission(Permission.FineLocation);
                }
            #endif

            kalmanCompass = new KalmanCompass(1f);

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

        void Update()
        {
            if (Input.location.status != LocationServiceStatus.Running)
                return;

            t += Time.deltaTime;

            // // Compass
            float heading = Input.compass.trueHeading;

            kalmanCompass.Process(heading, Input.compass.headingAccuracy, Input.compass.timestamp);

            if (t >= UPDATE_TIME)
            {
                t = 0;
                bool compassChanged = true;//!Math.Approximately(trueHeading, kalmanCompass.Heading);

                trueHeading = kalmanCompass.Heading;

                if (compassChanged)
                    compassChangedEvent?.Invoke(trueHeading);
            }
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
        }

        void StopLocationService()
        {
            // Stop service if there is no need to query location updates continuously
            Input.location.Stop();
        }
    }
}
