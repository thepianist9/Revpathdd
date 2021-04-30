using TMPro;
using UnityEngine;

public class CameraMatcher : MonoBehaviour
{
    public GameObject m_gpsUIText;

    private Transform m_mainCamera;

    void Awake()
    {
        m_mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
    }
 
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, m_mainCamera.transform.localEulerAngles.y, 0);
        // transform.rotation = GetComponent<Camera>().transform.rotation;

        m_gpsUIText.GetComponent<TMP_Text>().text =
                "camera localEulerAngles.y: " + m_mainCamera.transform.localEulerAngles.y;
    }
}
