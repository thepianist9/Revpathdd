using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    Transform m_mainCamera;

    void Awake()
    {
        m_mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            m_mainCamera.position.x,
            0,
            m_mainCamera.position.z
        );
    }
}
