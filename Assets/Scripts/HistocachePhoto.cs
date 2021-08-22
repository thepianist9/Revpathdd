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

        private Camera m_MainCamera;

        public MeshRenderer m_Quad;

        private string m_PhotoURL;

        private Renderer m_renderer;

        // Start is called before the first frame update
        void Start()
        {
            m_MainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 lookAtCamera = (m_MainCamera.transform.position - transform.position);
            lookAtCamera.y = 0;

            float viewingAngle = Vector3.Angle(transform.forward, lookAtCamera);

            // GameObject.Find("DebugText1").GetComponent<TMP_Text>().text = "Forward Vector: " + transform.forward;
            // GameObject.Find("DebugText2").GetComponent<TMP_Text>().text = "Vector To Camera: " + lookAtCamera;
            // GameObject.Find("DebugText3").GetComponent<TMP_Text>().text = "Angle: " + viewingAngle;

            float imageAlpha = 1f;
            if (viewingAngle > 35f && viewingAngle <= 75f)
                imageAlpha = 1f - (viewingAngle - 35f) / (75f - 35f);
            else if (viewingAngle > 75f)
                imageAlpha = 0f;

            Color c = m_Quad.material.color;
            c.a = imageAlpha;
            m_Quad.material.color = c;
        }

        public void SetPhotoURL(string url, float imageHeight, float aspectRatio, Transform lookAtTransform)
        {
            if (url.Equals(m_PhotoURL))
                return;

            Vector3 histocachePhotoLookPosition = lookAtTransform.position - transform.position;
            histocachePhotoLookPosition.y = 0;
            transform.rotation = Quaternion.LookRotation(histocachePhotoLookPosition);

            m_PhotoURL = url;

            m_Quad.transform.localScale = new Vector3(aspectRatio * imageHeight, imageHeight, 1f);

            Vector3 currentLocalPosition = m_Quad.transform.localPosition;
            currentLocalPosition.y = 0.5f * imageHeight;
            m_Quad.transform.localPosition = currentLocalPosition;

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
