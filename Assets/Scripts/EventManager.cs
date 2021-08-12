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

        public Button menuButton;

        public Button placesButton;
        public Button helpButton;
        public Button aboutButton;
        public Toggle languageToggle;
        //Animation Dropdown
        Vector2 posPlacesButton;
        Vector2 posAboutButton;
        Vector2 posHelpButton;
        Vector2 posLanguageButton;
        bool goingDown;
        bool goingUp;
        float animationSpeed;
        float animationProgress;
        // Places
        public Places places;

        // Help
        public Help help;

        // About
        public About about;

        // Start is called before the first frame update
        void Start()
        {
            menuButton.onClick.AddListener(OnMenu);

            placesButton.onClick.AddListener(OnPlaces);
            helpButton.onClick.AddListener(OnHelp);
            aboutButton.onClick.AddListener(OnAbout);

            posPlacesButton = placesButton.GetComponent<RectTransform>().anchoredPosition;
            posAboutButton = aboutButton.GetComponent<RectTransform>().anchoredPosition;
            posHelpButton = helpButton.GetComponent<RectTransform>().anchoredPosition;
            posLanguageButton = languageToggle.GetComponent<RectTransform>().anchoredPosition;
            goingDown = false;
            goingUp = true;
            animationSpeed = 6.0f;
        }

        void Update()
        {
            if (goingDown)
            {
                animationProgress += animationSpeed;
                placesButton.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(placesButton.GetComponent<RectTransform>().anchoredPosition,
                posPlacesButton, animationSpeed * Time.deltaTime);
                aboutButton.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(aboutButton.GetComponent<RectTransform>().anchoredPosition,
                posAboutButton, animationSpeed * Time.deltaTime);
                helpButton.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(helpButton.GetComponent<RectTransform>().anchoredPosition,
                posHelpButton, animationSpeed * Time.deltaTime);
                languageToggle.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(languageToggle.GetComponent<RectTransform>().anchoredPosition,
                posLanguageButton, animationSpeed * Time.deltaTime);

                menuButton.GetComponent<RectTransform>().localRotation = Quaternion.Lerp(menuButton.GetComponent<RectTransform>().localRotation, Quaternion.Euler(0, 0, 45f), animationSpeed * Time.deltaTime);
            }
            if (goingUp)
            {
                placesButton.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(placesButton.GetComponent<RectTransform>().anchoredPosition,
                Vector2.zero, animationSpeed * Time.deltaTime);
                aboutButton.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(aboutButton.GetComponent<RectTransform>().anchoredPosition,
                Vector2.zero, animationSpeed * Time.deltaTime);
                helpButton.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(helpButton.GetComponent<RectTransform>().anchoredPosition,
                Vector2.zero, animationSpeed * Time.deltaTime);
                languageToggle.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(languageToggle.GetComponent<RectTransform>().anchoredPosition,
                Vector2.zero, animationSpeed * Time.deltaTime);

                menuButton.GetComponent<RectTransform>().localRotation = Quaternion.Lerp(menuButton.GetComponent<RectTransform>().localRotation, Quaternion.identity, animationSpeed * Time.deltaTime);
            }
        }

        void Destroy()
        {
            menuButton.onClick.RemoveListener(OnMenu);

            placesButton.onClick.RemoveListener(OnPlaces);
            helpButton.onClick.RemoveListener(OnHelp);
            aboutButton.onClick.RemoveListener(OnAbout);
        }

        void OnMenu()
        {
            //Debug.Log("EventManager::Menu " + isOpen);
            if(goingDown){goingDown=false;goingUp=true;}
            else{goingDown=true;goingUp=false;}
            animationProgress = 0;

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
