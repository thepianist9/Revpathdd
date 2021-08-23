using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace HistocachingII
{
    public class ScreenManager : MonoBehaviour
    {
        public GameObject m_ARModeButton;

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

        public GameObject m_CameraStateUI;
        public GameObject m_MapStateUI;

        public ARSession m_ARSession;

        private const float m_ARSessionTimeout = 10f;

        private float m_DeltaTime;
        private bool m_Loading;

        private StateManager SM;

        public World m_World;

        private AROcclusionManager m_AROcclusionManager;

        void Awake()
        {
            SM = StateManager.Instance;

            m_DeltaTime = 0;
            m_Loading = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(CheckARAvailability());

            SM.OnStateChange += ChangeScreen;
            SM.SetState(State.Map);
            m_MainCamera = Camera.main;
            m_AROcclusionManager = m_MainCamera.GetComponent<AROcclusionManager>();
        }

        private IEnumerator CheckARAvailability()
        {
            if (ARSession.state == ARSessionState.None || ARSession.state == ARSessionState.CheckingAvailability)
            {
                yield return ARSession.CheckAvailability();
            }

            if (ARSession.state == ARSessionState.Unsupported)
            {
                // Start some fallback experience for unsupported devices
                m_ARModeButton.SetActive(false);
            }
            else
            {
                // Allow the AR session
                m_ARModeButton.SetActive(true);
            }    
        }

        // Update is called once per frame
        void Update()
        {
            if (m_Loading)
            {
                m_DeltaTime += Time.deltaTime;

                if (m_DeltaTime >= m_LoadingTime)
                {
                    m_Loading = false;

                    StartCoroutine("FadeOutCanvas", m_LoadingScreen);
                }
                else
                {
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
            m_CameraStateUI.SetActive(false);

            // Disable ARSession
            m_ARSession.enabled = false;

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

            m_MapStateUI.SetActive(true);

            StopCoroutine("ChangeToMapScreen");
        }

        IEnumerator ChangeToCameraScreen()
        {
            m_MapStateUI.SetActive(false);

            m_World.DestroyWorld();

            // Start a new ARSession
            m_ARSession.enabled = true;

            // Reset ARSession to reset world's axes
            m_ARSession.Reset();

            // TODO: change this into loading screen,
            //       instructing user to move around the device
            float time = 0;

            while (time < m_ARSessionTimeout && ARSession.state < ARSessionState.SessionTracking)
            {
                yield return null;
                time += Time.deltaTime;
            }

            if (ARSession.state < ARSessionState.SessionTracking)
            {
                SM.SetState(State.Map);
                yield break;
            }

            m_World.GenerateWorld();

            m_Minimap.SetActive(true);

            RectTransform minimapRectTransform = m_Minimap.GetComponent<RectTransform>();
            RectTransform minimapMaskRectTransform = m_MinimapMask.GetComponent<RectTransform>();

            m_MainCamera.enabled = true;
            m_MapCamera.enabled = false;

            minimapRectTransform.anchoredPosition = Vector3.zero;
            // m_Minimap.transform.parent = m_MinimapPosBottomLeft.transform;
            m_Minimap.transform.SetParent(m_MinimapPosBottomLeft.transform, false);

            time = 0;
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

            m_CameraStateUI.SetActive(true);

            StopCoroutine("ChangeToCameraScreen");
        }
  
        IEnumerator FadeOutCanvas(GameObject canvas)
        {
            while (canvas.GetComponent<CanvasGroup>().alpha > 0f)
            {
                canvas.GetComponent<CanvasGroup>().alpha -= Time.deltaTime;
                yield return null;
            }

            canvas.SetActive(false);

            StopCoroutine("FadeOutCanvas");
        }

        public void SwitchToMapScreen()
        {
            DataManager.Instance.Reset();
            SM.SetState(State.Map);
        }

        public void SwitchToCameraScreen()
        {
            DataManager.Instance.Reset();
            SM.SetState(State.Camera);
        }

        public void ToggleAROcclusion()
        {
            m_AROcclusionManager.enabled = !m_AROcclusionManager.enabled;
        }

    }
}
