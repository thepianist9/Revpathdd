using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HistocachingII
{
    public class NavigationManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            
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

        public void GoToMapScene()
        {
            // Use a coroutine to load the Scene in the background
            StartCoroutine(LoadAsyncMapScene());
        }

        IEnumerator LoadAsyncMapScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Map");

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}
