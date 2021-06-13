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

        public Button settingsButton;

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
            // settingsButton.onClick.AddListener(OnSettings);

            placesButton.onClick.AddListener(OnPlaces);
            helpButton.onClick.AddListener(OnHelp);
            aboutButton.onClick.AddListener(OnAbout);

            // helpBackButton.onClick.AddListener(OnHelpBack);
            // aboutBackButton.onClick.AddListener(OnAboutBack);
        }

        void Destroy()
        {
            // settingsButton.onClick.RemoveListener(OnSettings);

            placesButton.onClick.RemoveListener(OnPlaces);
            helpButton.onClick.RemoveListener(OnHelp);
            aboutButton.onClick.RemoveListener(OnAbout);

            // helpBackButton.onClick.RemoveListener(OnHelpBack);
            // aboutBackButton.onClick.RemoveListener(OnAboutBack);
        }

        void OnSettings()
        {
            Debug.Log("EventManager::Settings");
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
