using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class EventManager : MonoBehaviour
    {
        // Menu
        public Canvas menuCanvas;

        public Toggle menuToggle;

        public Button placesButton;
        public Button helpButton;
        public Button aboutButton;
        public Toggle languageToggle;

        // Expand / Collapse Animation
        private RectTransform placesRectTransform;
        private RectTransform helpRectTransform;
        private RectTransform aboutRectTransform;
        private RectTransform languageRectTransform;

        private Vector2 posPlacesButton;
        private Vector2 posAboutButton;
        private Vector2 posHelpButton;
        private Vector2 posLanguageButton;

        private const float animationSpeed = 2.5f;

        // Places
        public Places places;

        // Help
        public Help help;

        // About
        public About about;

        // Start is called before the first frame update
        void Start()
        {
            placesRectTransform = placesButton.GetComponent<RectTransform>();
            helpRectTransform = helpButton.GetComponent<RectTransform>();
            aboutRectTransform = aboutButton.GetComponent<RectTransform>();
            languageRectTransform = languageToggle.GetComponent<RectTransform>();

            posPlacesButton = placesRectTransform.anchoredPosition;
            posAboutButton = aboutRectTransform.anchoredPosition;
            posHelpButton = helpRectTransform.anchoredPosition;
            posLanguageButton = languageRectTransform.anchoredPosition;

            OnMenu(menuToggle.isOn);
            
            menuToggle.onValueChanged.AddListener(OnMenu);

            placesButton.onClick.AddListener(OnPlaces);
            helpButton.onClick.AddListener(OnHelp);
            aboutButton.onClick.AddListener(OnAbout);
        }

        // void Update()
        // {
        //     if (goingDown)
        //     {
        //         animationProgress += animationSpeed;
        //         placesButton.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(placesButton.GetComponent<RectTransform>().anchoredPosition,
        //         posPlacesButton, animationSpeed * Time.deltaTime);
        //         aboutButton.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(aboutButton.GetComponent<RectTransform>().anchoredPosition,
        //         posAboutButton, animationSpeed * Time.deltaTime);
        //         helpButton.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(helpButton.GetComponent<RectTransform>().anchoredPosition,
        //         posHelpButton, animationSpeed * Time.deltaTime);
        //         languageToggle.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(languageToggle.GetComponent<RectTransform>().anchoredPosition,
        //         posLanguageButton, animationSpeed * Time.deltaTime);

        //         menuButton.GetComponent<RectTransform>().localRotation = Quaternion.Lerp(menuButton.GetComponent<RectTransform>().localRotation, Quaternion.Euler(0, 0, 45f), animationSpeed * Time.deltaTime);
        //     }
        //     if (goingUp)
        //     {
        //         placesButton.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(placesButton.GetComponent<RectTransform>().anchoredPosition,
        //         Vector2.zero, animationSpeed * Time.deltaTime);
        //         aboutButton.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(aboutButton.GetComponent<RectTransform>().anchoredPosition,
        //         Vector2.zero, animationSpeed * Time.deltaTime);
        //         helpButton.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(helpButton.GetComponent<RectTransform>().anchoredPosition,
        //         Vector2.zero, animationSpeed * Time.deltaTime);
        //         languageToggle.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(languageToggle.GetComponent<RectTransform>().anchoredPosition,
        //         Vector2.zero, animationSpeed * Time.deltaTime);

        //         menuButton.GetComponent<RectTransform>().localRotation = Quaternion.Lerp(menuButton.GetComponent<RectTransform>().localRotation, Quaternion.identity, animationSpeed * Time.deltaTime);
        //     }
        // }

        private IEnumerator SmoothDamp(RectTransform rectTransform, Vector2 target, float speed)
        {
            float factor = 0;

            // Adapted from Paul's code: constant, dampened movement towards the target, similar to SmoothDamp.
            // It gets closer and closer to the target in ever-decreasing amounts, the while loop is added to finish the movement.
            // SmoothDamp function, not sure why, does not create the desired effect, hence we continue to use Lerp. 
            while (factor < 1f)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, target, factor);

                yield return null;

                factor += speed * Time.deltaTime;
            }

            rectTransform.anchoredPosition = target;
        }

        void Destroy()
        {
            menuToggle.onValueChanged.RemoveListener(OnMenu);

            placesButton.onClick.RemoveListener(OnPlaces);
            helpButton.onClick.RemoveListener(OnHelp);
            aboutButton.onClick.RemoveListener(OnAbout);
        }

        void OnMenu(bool isOn)
        {
            //Debug.Log("EventManager::Menu " + isOpen);
            // if(goingDown){goingDown=false;goingUp=true;}
            // else{goingDown=true;goingUp=false;}
            // animationProgress = 0;
            
            if (isOn)
            {
                StartCoroutine(SmoothDamp(languageRectTransform, posLanguageButton, animationSpeed));
                StartCoroutine(SmoothDamp(helpRectTransform, posHelpButton, animationSpeed));
                StartCoroutine(SmoothDamp(aboutRectTransform, posAboutButton, animationSpeed));
                StartCoroutine(SmoothDamp(placesRectTransform, posPlacesButton, animationSpeed));
            }
            else
            {
                StartCoroutine(SmoothDamp(languageRectTransform, Vector2.zero, animationSpeed));
                StartCoroutine(SmoothDamp(helpRectTransform, Vector2.zero, animationSpeed));
                StartCoroutine(SmoothDamp(aboutRectTransform, Vector2.zero, animationSpeed));
                StartCoroutine(SmoothDamp(placesRectTransform, Vector2.zero, animationSpeed));
            }

            //placesButton.gameObject.SetActive(isOpen);
            //helpButton.gameObject.SetActive(isOpen);
            //aboutButton.gameObject.SetActive(isOpen);
            //languageToggle.gameObject.SetActive(isOpen);
        }

        void OnPlaces()
        {
            Debug.Log("EventManager::Places");

            places.Show(languageToggle.isOn ? 1 : 0);
        }

        void OnHelp()
        {
            Debug.Log("EventManager::Help");

            help.Show(languageToggle.isOn ? 1 : 0);
        }

        void OnAbout()
        {
            Debug.Log("EventManager::About");

            about.Show(languageToggle.isOn ? 1 : 0);
        }

        void OnLanguage()
        {
            Debug.Log("EventManager::Language");

            // TODO show toast "Sprache auf Englisch geändert" / "Language changed into English"
        }
    }
}
