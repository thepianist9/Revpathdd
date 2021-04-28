using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

public class LocationService : MonoBehaviour
{
    public const float UPDATE_TIME = 0.5f;

    public GameObject m_gpsUIText;

    private IEnumerator coroutine;

    void Awake()
    {
        #if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }
        #endif
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
        // else
        // {
        //     // Access granted and location value could be retrieved
        //     string info = "Latitude: " + Input.location.lastData.latitude + "\n" +
        //         "Longitude: " + Input.location.lastData.longitude + "\n" +
        //         "Altitude: " + Input.location.lastData.altitude + "\n" +
        //         "Hor Accuracy: " + Input.location.lastData.horizontalAccuracy + "\n" +
        //         "Timestamp: " + Input.location.lastData.timestamp;

        //     m_gpsUIText.GetComponent<UnityEngine.UI.Text>().text = info;
        // }

        WaitForSeconds updateTime = new WaitForSeconds(UPDATE_TIME);

        while (true)
        {
            string info = "Latitude: " + Input.location.lastData.latitude + "\n" +
                "Longitude: " + Input.location.lastData.longitude + "\n" +
                "Altitude: " + Input.location.lastData.altitude + "\n" +
                "Hor Accuracy: " + Input.location.lastData.horizontalAccuracy + "\n" +
                "Timestamp: " + Input.location.lastData.timestamp;

                m_gpsUIText.GetComponent<TMP_Text>().text = info;

                yield return updateTime;
        }
    }

    void StopLocationService()
    {
        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }
}
