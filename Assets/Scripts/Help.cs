using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class Help : MonoBehaviour
    {
        private static readonly string[] titles = { "Wie benutzt man?",
                                                    "How to use?" };

        private static readonly string[] captions = { "Stasi an der TU Dresden. Eine virtuelle Spurensuche. This page will explain on how to use the app (in German).",
                                                      "The Stasi at the TU Dresden. Tracing Clues Virtually. This page will explain on how to use the app (in English)." };

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
            Debug.Log("Help::Show " + language);

            titleText.text = titles[language];
            captionText.text = captions[language];

            canvas.enabled = true;
        }

        public void Hide()
        {
            Debug.Log("Help::Hide");

            canvas.enabled = false;
        }
    }
}
