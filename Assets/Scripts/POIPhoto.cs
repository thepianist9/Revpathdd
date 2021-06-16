using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class POIPhoto : MonoBehaviour
    {
        public Texture2D loading, error;

        private Camera m_MainCamera;

        public MeshRenderer m_Quad;

        private string m_PhotoURL;

        // Start is called before the first frame update
        void Start()
        {
            m_MainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            // TODO temporary while we have not anchored this
            Vector3 lookPosition = transform.position - m_MainCamera.transform.position;
            lookPosition.y = 0;
            transform.rotation = Quaternion.LookRotation(lookPosition);
        }

        public void SetPhotoURL(string url, float aspectRatio)
        {
            if (url.Equals(m_PhotoURL))
                return;

            m_PhotoURL = url;
            
            m_Quad.transform.localScale = new Vector3(15, aspectRatio * 15, 1);

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
