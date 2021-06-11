using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HistocachingII
{
    public class NavigationManager : MonoBehaviour
    {
        public Camera m_MapCamera;

        private Camera m_MainCamera;

        private StateManager SM;

        void Awake()
        {
            SM = StateManager.Instance;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_MainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            // if (deviceOrientation != Input.deviceOrientation)
            // {
            //     deviceOrientation = Input.deviceOrientation;

            //     if (deviceOrientation == DeviceOrientation.Portrait)
            //     {
            //         GoToMainScene();
            //     }
            //     else if (deviceOrientation == DeviceOrientation.FaceUp)
            //     {
            //         GoToMapScene();
            //     }
            // }
        }

        public void GoToMainScene()
        {
            // Use a coroutine to load the Scene in the background
            StartCoroutine(LoadAsyncMainScene());
        }
        IEnumerator LoadAsyncMainScene()
        {
            // The Application loads the Scene in the background as the current Scene runs.
            // This is particularly good for creating loading screens.
            // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
            // a sceneBuildIndex of 1 as shown in Build Settings.

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main");

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        public void GoToMainMapboxEMAScene()
        {
            StartCoroutine(LoadAsyncMainMapboxEMAScene());
        }
        IEnumerator LoadAsyncMainMapboxEMAScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MapboxEMA");
            while (!asyncLoad.isDone)
                yield return null;
        }
        public void GoToMainMapboxAverageScene()
        {
            StartCoroutine(LoadAsyncMainMapboxAverageScene());
        }
        IEnumerator LoadAsyncMainMapboxAverageScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MapboxAverage");
            while (!asyncLoad.isDone)
                yield return null;
        }
        public void GoToMainMapboxLowPassScene()
        {
            StartCoroutine(LoadAsyncMainMapboxLowPassScene());
        }
        IEnumerator LoadAsyncMainMapboxLowPassScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MapboxLowPass");
            while (!asyncLoad.isDone)
                yield return null;
        }
        public void GoToMainMapboxAndroidScene()
        {
            StartCoroutine(LoadAsyncMainMapboxAndroidScene());
        }
        IEnumerator LoadAsyncMainMapboxAndroidScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MapboxAndroid");
            while (!asyncLoad.isDone)
                yield return null;
        }

        public void GoToMapScene()
        {
            StartCoroutine(LoadAsyncMapScene());
        }
        IEnumerator LoadAsyncMapScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Map");
            while (!asyncLoad.isDone)
                yield return null;
        }

        public void ChangeCamera()
        {
            if (m_MainCamera.enabled)
            {
                m_MainCamera.enabled = false;
                m_MapCamera.enabled = true;

                // SM.SetState(State.Map);
            }
            else
            {
                m_MainCamera.enabled = true;
                m_MapCamera.enabled = false;

                // SM.SetState(State.Camera);
            }
        }
    }
}
