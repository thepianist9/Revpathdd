using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HistocachingII
{
    public class PoiDistance : MonoBehaviour
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
            transform.rotation = Quaternion.LookRotation(transform.position - m_mainCamera.position);
            // float distance = Mathf.Floor(Vector3.Distance(transform.position, m_mainCamera.position));
            // GetComponent<TMP_Text>().text = distance + "m";
        }
    }
}
