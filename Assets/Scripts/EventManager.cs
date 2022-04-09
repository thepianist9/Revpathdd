using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class EventManager : MonoBehaviour
    {
        // Language changed titles
        private static readonly string[] languageChangedTitles = { "Sprache auf Deutsch geändert", "Language changed into English" };

        // Menu
        public Canvas menuCanvas;

        public Toggle menuToggle;

        public Button galleryButton;
        public Button helpButton;
        public Button aboutButton;
        public Toggle languageToggle;
        public Button tourButton;

        // Expand / Collapse Animation
        private RectTransform galleryRectTransform;
        private RectTransform helpRectTransform;
        private RectTransform aboutRectTransform;
        private RectTransform languageRectTransform;
        private RectTransform tourRectTransform;

        private Vector2 posGalleryButton;
        private Vector2 posAboutButton;
        private Vector2 posHelpButton;
        private Vector2 posLanguageButton;
        private Vector2 posTourButton;


        private const float animationSpeed = 2.5f;

        // Gallery
        public Gallery gallery;

        // Help
        public Help help;

        // About
        public About about;

        // Toast
        public Toast toast;

        public Tour tour;

        // Start is called before the first frame update
        void Start()
        {
            galleryRectTransform = galleryButton.GetComponent<RectTransform>();
            helpRectTransform = helpButton.GetComponent<RectTransform>();
            aboutRectTransform = aboutButton.GetComponent<RectTransform>();
            languageRectTransform = languageToggle.GetComponent<RectTransform>();
            tourRectTransform = tourButton.GetComponent<RectTransform>();

            posGalleryButton = galleryRectTransform.anchoredPosition;
            posHelpButton = helpRectTransform.anchoredPosition;
            posAboutButton = aboutRectTransform.anchoredPosition;
            posLanguageButton = languageRectTransform.anchoredPosition;
            posTourButton = tourRectTransform.anchoredPosition;

            OnMenu(menuToggle.isOn);

            menuToggle.onValueChanged.AddListener(OnMenu);

            galleryButton.onClick.AddListener(OnGallery);
            helpButton.onClick.AddListener(OnHelp);
            aboutButton.onClick.AddListener(OnAbout);
            tourButton.onClick.AddListener(OnTour);

            languageToggle.onValueChanged.AddListener(OnLanguageChanged);
        }

        void Destroy()
        {
            menuToggle.onValueChanged.RemoveListener(OnMenu);

            galleryButton.onClick.RemoveListener(OnGallery);
            helpButton.onClick.RemoveListener(OnHelp);
            aboutButton.onClick.RemoveListener(OnAbout);

            languageToggle.onValueChanged.RemoveListener(OnLanguageChanged);
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

        public void OnMenu(bool isOn)
        {
            Debug.Log("EventManager::Menu " + isOn);
            // if(goingDown){goingDown=false;goingUp=true;}
            // else{goingDown=true;goingUp=false;}
            // animationProgress = 0;

            if (isOn)
            {
                StartCoroutine(SmoothDamp(languageRectTransform, posLanguageButton, animationSpeed));
                StartCoroutine(SmoothDamp(helpRectTransform, posHelpButton, animationSpeed));
                StartCoroutine(SmoothDamp(aboutRectTransform, posAboutButton, animationSpeed));
                StartCoroutine(SmoothDamp(galleryRectTransform, posGalleryButton, animationSpeed));
                StartCoroutine(SmoothDamp(tourRectTransform, posTourButton, animationSpeed));
            }
            else
            {
                StartCoroutine(SmoothDamp(languageRectTransform, Vector2.zero, animationSpeed));
                StartCoroutine(SmoothDamp(helpRectTransform, Vector2.zero, animationSpeed));
                StartCoroutine(SmoothDamp(aboutRectTransform, Vector2.zero, animationSpeed));
                StartCoroutine(SmoothDamp(galleryRectTransform, Vector2.zero, animationSpeed));
                StartCoroutine(SmoothDamp(tourRectTransform, Vector2.zero, animationSpeed));
            }

            //placesButton.gameObject.SetActive(isOpen);
            //helpButton.gameObject.SetActive(isOpen);
            //aboutButton.gameObject.SetActive(isOpen);
            //languageToggle.gameObject.SetActive(isOpen);
        }

        public void OnGallery()
        {
            Debug.Log("EventManager::Gallery");

            gallery.gameObject.SetActive(true);
            gallery.Show(languageToggle.isOn ? 1 : 0);
        }

        public void OnHelp()
        {
            Debug.Log("EventManager::Help");

            help.gameObject.SetActive(true);
            help.Show(languageToggle.isOn ? 1 : 0);
        }

        public void OnAbout()
        {
            Debug.Log("EventManager::About");

            about.gameObject.SetActive(true);
            about.Show(languageToggle.isOn ? 1 : 0);
        }

        public void OnTour()
        {
            Debug.Log("EventManager::Tour");   
            tour.gameObject.SetActive(true);
            tour.Show(languageToggle.isOn ? 1 : 0);
        }

        public void OnLanguageChanged(bool on)
        {
            Debug.Log("EventManager::Language");

            toast.gameObject.SetActive(true);
            toast.Show(languageToggle.isOn ? languageChangedTitles[1] : languageChangedTitles[0]);
        }
    }
}
