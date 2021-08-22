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

        // Animation Dropdown
        private RectTransform placesRectTransform;
        private RectTransform helpRectTransform;
        private RectTransform aboutRectTransform;
        private RectTransform languageRectTransform;

        private Vector2 posPlacesButton;
        private Vector2 posAboutButton;
        private Vector2 posHelpButton;
        private Vector2 posLanguageButton;

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

        private IEnumerator Expand(float duration)
        {
            float time = 0;

            while (time < duration)
            {
                placesRectTransform.anchoredPosition = Vector2.Lerp(placesRectTransform.anchoredPosition, posPlacesButton, time);
                aboutRectTransform.anchoredPosition = Vector2.Lerp(aboutRectTransform.anchoredPosition, posAboutButton, time);
                helpRectTransform.anchoredPosition = Vector2.Lerp(helpRectTransform.anchoredPosition, posHelpButton, time);
                languageRectTransform.anchoredPosition = Vector2.Lerp(languageRectTransform.anchoredPosition, posLanguageButton, time);

                time += Time.deltaTime;
                yield return null;
            }

            placesRectTransform.anchoredPosition = posPlacesButton;
            aboutRectTransform.anchoredPosition = posAboutButton;
            helpRectTransform.anchoredPosition = posHelpButton;
            languageRectTransform.anchoredPosition = posLanguageButton;
        }

        private IEnumerator Collapse(float duration)
        {
            float time = 0;

            while (time < duration)
            {
                placesRectTransform.anchoredPosition = Vector2.Lerp(placesRectTransform.anchoredPosition, Vector2.zero, time);
                aboutRectTransform.anchoredPosition = Vector2.Lerp(aboutRectTransform.anchoredPosition, Vector2.zero, time);
                helpRectTransform.anchoredPosition = Vector2.Lerp(helpRectTransform.anchoredPosition, Vector2.zero, time);
                languageRectTransform.anchoredPosition = Vector2.Lerp(languageRectTransform.anchoredPosition, Vector2.zero, time);

                time += Time.deltaTime;
                yield return null;
            }

            placesRectTransform.anchoredPosition = Vector2.zero;
            aboutRectTransform.anchoredPosition = Vector2.zero;
            helpRectTransform.anchoredPosition = Vector2.zero;
            languageRectTransform.anchoredPosition = Vector2.zero;
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
            
            StartCoroutine(isOn ? Expand(0.25f) : Collapse(0.25f));

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
