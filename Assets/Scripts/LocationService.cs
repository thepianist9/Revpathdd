using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class LocationService : MonoBehaviour
{
    public GameObject m_gpsUIText;

    IEnumerator Start()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        // Start service before querying location
        Input.location.Start();

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
        else
        {
            // Access granted and location value could be retrieved
            string info = "Latitude: " + Input.location.lastData.latitude + "\n" +
                "Longitude: " + Input.location.lastData.longitude + "\n" +
                "Altitude: " + Input.location.lastData.altitude + "\n" +
                "Hor Accuracy: " + Input.location.lastData.horizontalAccuracy + "\n" +
                "Timestamp: " + Input.location.lastData.timestamp;

            m_gpsUIText.GetComponent<UnityEngine.UI.Text>().text = info;
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }

        // #if UNITY_ANDROID
        //     if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        //     {
        //         Permission.RequestUserPermission(Permission.FineLocation);
        //     }
        // #endif
}
