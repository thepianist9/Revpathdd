using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class About : MonoBehaviour
    {
        private static readonly string[] titles = { "Stasi an der TU Dresden.\nEine virtuelle Spurensuche.",
                                                    "The Stasi at the TU Dresden.\nTracing Clues Virtually." };

        private static readonly string[] captions = { "Stasi an der TU Dresden. Eine virtuelle Spurensuche. This page will describe about the app, stakeholders, credits, contact emails, etc (in German).",
                                                      "The Stasi at the TU Dresden. Tracing Clues Virtually. This page will describe about the app, stakeholders, credits, contact emails, etc (in English)." };

        // UI
        public Canvas canvas;
    
        public Button backButton;
        public Text titleText;
        public Text captionText;

        // Start is called before the first frame update
        void Start()
        {
            backButton.onClick.AddListener(Hide);
        }

        public void Show(int language)
        {
            Debug.Log("About::Show " + language);

            titleText.text = titles[language];
            captionText.text = captions[language];

            canvas.enabled = true;
        }

        public void Hide()
        {
            Debug.Log("About::Hide");

            canvas.enabled = false;
        }
    }
}
