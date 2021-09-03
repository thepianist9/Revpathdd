using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
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
        public Transform m_MapPlayerTransform;

        public bool m_IsStickyMyLocation = true;
        public GameObject m_MinimapPosCenter;
        public GameObject m_MinimapPosBottomLeft;
        public GameObject m_Minimap;
        public GameObject m_MinimapMask;
        public GameObject m_MyLocationTop;

        public float m_LoadingTime = 7.0f;
        public GameObject m_LoadingScreen;
        public GameObject m_LoadingBar;

        public GameObject m_CameraStateUI;
        public GameObject m_MapStateUI;

        public ARSession m_ARSession;

        private const float m_ARSessionTimeout = 10f;

        private StateManager SM;

        public World m_World;

        private AROcclusionManager m_AROcclusionManager;

        private bool m_LocationAvailable;
        private bool m_ARSupported;

        private ILocationProvider _locationProvider;

        void Awake()
        {
            // Test battery and RAM usage
            Application.targetFrameRate = 30;

            SM = StateManager.Instance;
        }

        // Start is called before the first frame update
        protected virtual IEnumerator Start()
        {
            yield return null;
			_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
			_locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;

            SM.OnStateChange += ChangeScreen;
            SM.SetState(State.Map);
            m_MainCamera = Camera.main;
            m_AROcclusionManager = m_MainCamera.GetComponent<AROcclusionManager>();

            yield return CheckLocationService();
            yield return CheckARAvailability();
        }

		void LocationProvider_OnLocationUpdated(Mapbox.Unity.Location.Location location)
		{
			_locationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
            m_LocationAvailable = true;
		}

        private IEnumerator CheckLocationService()
        {
            float deltaTime = 0;
            float xScale = 0;

            RectTransform rectTransform = m_LoadingBar.GetComponent<RectTransform>();

            while (!m_LocationAvailable)
            {
                deltaTime += Time.deltaTime;
                xScale = Mathf.Min(deltaTime / m_LoadingTime, 0.9f);
                
                rectTransform.localScale = new Vector3(xScale, 1f, 1f);

                yield return null;
            }

            while (xScale < 1f)
            {
                xScale += 0.1f;
                
                if (xScale > 1f)
                    xScale = 1f;

                rectTransform.localScale = new Vector3(xScale, 1f, 1f);

                yield return null;
            }

            StartCoroutine("FadeOutCanvas", m_LoadingScreen);
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
                m_ARSupported = false;
                // m_ARModeButton.SetActive(false);
            }
            else
            {
                // Allow the AR session
                m_ARSupported = true;
                // m_ARModeButton.SetActive(true);
            }    
        }

        // Update is called once per frame
        // void Update()
        // {
        //     if (m_Loading)
        //     {
        //         m_DeltaTime += Time.deltaTime;

        //         if (m_DeltaTime >= m_LoadingTime)
        //         {
        //             m_Loading = false;

        //             StartCoroutine("FadeOutCanvas", m_LoadingScreen);
        //         }
        //         else
        //         {
        //             float xScale = m_DeltaTime / m_LoadingTime;
        //             if (xScale < 0.9f)
        //             {
        //                 if (Random.Range(1, 10) >= 9)
        //                     m_LoadingBar.GetComponent<RectTransform>().localScale = new Vector3(xScale, 1f, 1f);
        //             }
        //             else
        //             {
        //                 m_LoadingBar.GetComponent<RectTransform>().localScale = new Vector3(xScale, 1f, 1f);
        //             }
        //         }
        //     }
        // }

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
            // m_ARSession.enabled = false;

            RectTransform minimapRectTransform = m_Minimap.GetComponent<RectTransform>();
            RectTransform minimapMaskRectTransform = m_MinimapMask.GetComponent<RectTransform>();

            minimapRectTransform.anchoredPosition = Vector3.zero;
            // m_Minimap.transform.parent = m_MinimapPosCenter.transform;
            m_Minimap.transform.SetParent(m_MinimapPosCenter.transform, false);

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

            m_World.DestroyWorld();

            // StopCoroutine("ChangeToMapScreen");
        }

        IEnumerator ChangeToCameraScreen()
        {
            m_MapStateUI.SetActive(false);

            // m_World.DestroyWorld();

            // Start a new ARSession
            // m_ARSession.enabled = true;

            // Reset ARSession to reset world's axes
            // m_ARSession.Reset();

            // TODO: change this into loading screen,
            //       instructing user to move around the device
            float time = 0;

            // while (time < m_ARSessionTimeout && ARSession.state < ARSessionState.SessionTracking)
            // {
            //     yield return null;
            //     time += Time.deltaTime;
            // }

            // if (ARSession.state < ARSessionState.SessionTracking)
            // {
            //     SM.SetState(State.Map);
            //     yield break;
            // }

            yield return m_World.GenerateWorld();

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

            // StopCoroutine("ChangeToCameraScreen");
        }
  
        IEnumerator FadeOutCanvas(GameObject canvas)
        {
            CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();

            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= Time.deltaTime;
                yield return null;
            }

            canvas.SetActive(false);

            // StopCoroutine("FadeOutCanvas");
        }

        public void SwitchToMapScreen()
        {
            m_MinimapCamera.GetComponent<FollowTarget>().target = m_MapCamera.gameObject.transform;
            DataManager.Instance.Reset();
            SM.SetState(State.Map);
        }

        public void SwitchToCameraScreen()
        {
            m_MinimapCamera.GetComponent<FollowTarget>().target = m_MapPlayerTransform;
            DataManager.Instance.Reset();
            SM.SetState(State.Camera);
        }

        public void ToggleAROcclusion()
        {
            m_AROcclusionManager.enabled = !m_AROcclusionManager.enabled;
        }

        public void ToggleStickyMyLocation()
        {
            m_IsStickyMyLocation = !m_IsStickyMyLocation;
            m_MyLocationTop.SetActive(m_IsStickyMyLocation);
        }

        public void SetStickyMyLocation(bool state)
        {
            m_IsStickyMyLocation = state;
            m_MyLocationTop.SetActive(m_IsStickyMyLocation);
        }
    }
}
