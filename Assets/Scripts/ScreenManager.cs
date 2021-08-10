using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HistocachingII
{
    public class ScreenManager : MonoBehaviour
    {
        private Camera m_MainCamera;
        public Camera m_MapCamera;
        public Camera m_MinimapCamera;

        public GameObject m_MinimapPosCenter;
        public GameObject m_MinimapPosBottomLeft;
        public GameObject m_Minimap;
        public GameObject m_MinimapMask;

        public float m_LoadingTime = 7.0f;
        public GameObject m_LoadingScreen;
        public GameObject m_LoadingBar;

        private float m_DeltaTime;
        private bool m_Loading;

        private StateManager SM;

        void Awake()
        {
            SM = StateManager.Instance;

            m_DeltaTime = 0;
            m_Loading = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            SM.OnStateChange += ChangeScreen;
            m_MainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_Loading)
            {
                m_DeltaTime += Time.deltaTime;

                float xScale = m_DeltaTime / m_LoadingTime;
                if (xScale < 0.9f)
                {
                    if (Random.Range(1, 10) >= 9)
                        m_LoadingBar.GetComponent<RectTransform>().localScale = new Vector3(xScale, 1f, 1f);
                }
                else
                {
                    m_LoadingBar.GetComponent<RectTransform>().localScale = new Vector3(xScale, 1f, 1f);
                }

                if (m_DeltaTime >= m_LoadingTime)
                {
                    m_Loading = false;

                    m_LoadingScreen.SetActive(false);

                    SM.SetState(State.Map);
                }
            }
        }

        void OnDisable()
        {
            SM.OnStateChange -= ChangeScreen;
        }

        void ChangeScreen()
        {
            StopCoroutine("ChangeToMapScreen");
            StopCoroutine("ChangeToCameraScreen");

            switch (SM.state)
            {
                case State.Map:
                    StartCoroutine("ChangeToMapScreen");
                    break;
                case State.Camera:
                    StartCoroutine("ChangeToCameraScreen");
                    break;
            }
        }

        IEnumerator ChangeToMapScreen()
        {
            RectTransform minimapRectTransform = m_Minimap.GetComponent<RectTransform>();
            RectTransform minimapMaskRectTransform = m_MinimapMask.GetComponent<RectTransform>();

            minimapRectTransform.anchoredPosition = Vector3.zero;
            m_Minimap.transform.parent = m_MinimapPosCenter.transform;

            float time = 0;
            Vector3 startPosition = minimapRectTransform.anchoredPosition;
            Vector2 startSize = new Vector2(300f, 300f);
            Vector2 targetSize = new Vector2(2048f, 2048f);
            float duration = 0.3f;

            while (time < duration)
            {
                minimapRectTransform.anchoredPosition = Vector3.Lerp(startPosition, Vector3.zero, time / duration);
                minimapMaskRectTransform.sizeDelta = Vector2.Lerp(startSize, targetSize, time / duration);
                m_MinimapCamera.transform.localPosition = Vector3.Lerp(new Vector3(0f, 0f, -200f), Vector3.zero, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            minimapRectTransform.anchoredPosition = Vector3.zero;
            minimapMaskRectTransform.sizeDelta = targetSize;
            m_MinimapCamera.transform.localPosition = Vector3.zero;

            m_MainCamera.enabled = false;
            m_MapCamera.enabled = true;

            m_Minimap.SetActive(false);

            StopCoroutine("ChangeToMapScreen");
        }

        IEnumerator ChangeToCameraScreen()
        {
            m_Minimap.SetActive(true);

            RectTransform minimapRectTransform = m_Minimap.GetComponent<RectTransform>();
            RectTransform minimapMaskRectTransform = m_MinimapMask.GetComponent<RectTransform>();

            m_MainCamera.enabled = true;
            m_MapCamera.enabled = false;

            minimapRectTransform.anchoredPosition = Vector3.zero;
            // m_Minimap.transform.parent = m_MinimapPosBottomLeft.transform;
            m_Minimap.transform.SetParent(m_MinimapPosBottomLeft.transform, false);

            float time = 0;
            Vector3 startPosition = minimapRectTransform.anchoredPosition;
            Vector2 startSize = new Vector2(2048f, 2048f);
            Vector2 targetMaskSize = new Vector2(300f, 300f);
            float duration = 0.3f;

            while (time < duration)
            {
                minimapRectTransform.anchoredPosition = Vector3.Lerp(startPosition, Vector3.zero, time / duration);
                minimapMaskRectTransform.sizeDelta = Vector2.Lerp(startSize, targetMaskSize, time / duration);
                m_MinimapCamera.transform.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0f, 0f, -200f), time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            minimapRectTransform.anchoredPosition = Vector3.zero;
            minimapMaskRectTransform.sizeDelta = targetMaskSize;
            m_MinimapCamera.transform.localPosition = new Vector3(0f, 0f, -200f);

            StopCoroutine("ChangeToCameraScreen");
        }
    }
}
