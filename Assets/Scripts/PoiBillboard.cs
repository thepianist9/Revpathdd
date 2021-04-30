using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoiBillboard : MonoBehaviour
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
        Vector3 lookPosition = transform.position - m_mainCamera.position;
        lookPosition.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPosition);
    }
}
