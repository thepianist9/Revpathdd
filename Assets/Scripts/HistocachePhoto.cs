using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class HistocachePhoto : MonoBehaviour
    {
        public Texture2D loading, error;

        public MeshRenderer m_Quad;

        public bool m_IsTypeB;

        private Camera m_MainCamera;

        private string m_PhotoURL;

        private Renderer m_renderer;

        // Start is called before the first frame update
        void Start()
        {
            // somehow does not work on iPhone
            // m_MainCamera = Camera.main;
            m_MainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!m_IsTypeB)
            {
                Vector3 lookAtCamera = (m_MainCamera.transform.position - transform.position);
                lookAtCamera.y = 0;

                float viewingAngle = Vector3.Angle(transform.forward, lookAtCamera);

                // GameObject.Find("DebugText1").GetComponent<TMP_Text>().text = "Forward Vector: " + transform.forward;
                // GameObject.Find("DebugText2").GetComponent<TMP_Text>().text = "Vector To Camera: " + lookAtCamera;
                // GameObject.Find("DebugText3").GetComponent<TMP_Text>().text = "Angle: " + viewingAngle;

                float imageAlpha = 1f;
                if (viewingAngle > 25f && viewingAngle <= 75f)
                    imageAlpha = 1f - (viewingAngle - 25f) / 50f;
                else if (viewingAngle > 75f)
                    imageAlpha = 0f;

                Color c = m_Quad.material.color;
                c.a = imageAlpha;
                m_Quad.material.color = c;
            }
        }

        public void SetPhotoURL(string url, float imageHeight, float aspectRatio, float imageOffset)
        {
            if (url.Equals(m_PhotoURL))
                return;

            m_PhotoURL = url;

            if (m_IsTypeB)
            {
                m_Quad.transform.localScale = new Vector3(aspectRatio * 1.5f, 1.5f, 1f);
            }
            else
            {
                transform.position += transform.forward * imageOffset;

                m_Quad.transform.localScale = new Vector3(aspectRatio * imageHeight, imageHeight, 1f);

                Vector3 currentLocalPosition = m_Quad.transform.localPosition;
                currentLocalPosition.y = 0.5f * imageHeight;
                m_Quad.transform.localPosition = currentLocalPosition;
            }

            if (m_Quad.material != null && m_Quad.material.mainTexture != null)
            {
                DestroyImmediate(m_Quad.material.mainTexture, true);
                
                m_Quad.material.mainTexture = null;
            }

            Davinci.get()
                .load(m_PhotoURL)
                .setLoadingPlaceholder(loading)
                .setErrorPlaceholder(error)
                .setFadeTime(0)
                .into(m_Quad)
                .start();
        }
    }
}
