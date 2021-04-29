using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class User : MonoBehaviour
{
    Transform m_mainCamera;
    // Transform m_compass;

    public const float UPDATE_TIME = 0.5f;

    public GameObject m_gpsUIText;

    public GameObject m_compass;

    private IEnumerator coroutine;

    void Awake()
    {
        // Input.compass.enabled = true;
        // Input.location.Start();
        // // m_compass = transform.Find("Compass").transform;

        m_mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        coroutine = StartLocationService();

        StartCoroutine(coroutine);
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
            m_gpsUIText.GetComponent<TMP_Text>().text = "trueHeading " + Input.compass.trueHeading;

            m_compass.transform.rotation = Quaternion.Euler(0, -Input.compass.trueHeading, 0);

            yield return updateTime;
        }
    }

    // Update is called once per frame
    // void Update()
    // {
    //     // transform.position = new Vector3(
    //     //     m_mainCamera.position.x,
    //     //     0,
    //     //     m_mainCamera.position.z
    //     // );

    //     // Orient compass to point northward
    //     m_compass.transform.rotation = Quaternion.Euler(0, -Input.compass.trueHeading, 0);
    // }
}
