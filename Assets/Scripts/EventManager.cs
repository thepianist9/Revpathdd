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

        // Places
        public Places places;

        // Help
        public Help help;

        // About
        public About about;

        // Start is called before the first frame update
        void Start()
        {
            menuToggle.onValueChanged.AddListener(OnMenu);

            placesButton.onClick.AddListener(OnPlaces);
            helpButton.onClick.AddListener(OnHelp);
            aboutButton.onClick.AddListener(OnAbout);
        }

        void Destroy()
        {
            menuToggle.onValueChanged.RemoveListener(OnMenu);

            placesButton.onClick.RemoveListener(OnPlaces);
            helpButton.onClick.RemoveListener(OnHelp);
            aboutButton.onClick.RemoveListener(OnAbout);
        }

        void OnMenu(bool isOpen)
        {
            Debug.Log("EventManager::Menu " + isOpen);

            placesButton.gameObject.SetActive(isOpen);
            helpButton.gameObject.SetActive(isOpen);
            aboutButton.gameObject.SetActive(isOpen);
            languageToggle.gameObject.SetActive(isOpen);
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
