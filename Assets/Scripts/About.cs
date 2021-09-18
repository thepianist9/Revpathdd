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

        private static readonly string[] captions = { "Diese App ermöglicht es ihnen, die Welt der DDR virtuell zu erleben. Sie wurde von Studenten als Teil eines Gruppenprojektes der Professur für Computergraphik und Visualisierung an der Technischen Universität Dresden entwickelt.\nEntwickler:\nDavid Victor Raj Anthony\nJan Zimmermann?\nMasoud Taghikhah\nOlena Horokh\nPaul Hunt\nSneha Verma Prakash\nTania Krisanty\nTanzila Sadika\nVictor\nBenutzte Technologien sind Unity, MapBox, ARCore/ARFoundation.",
                                                      "This app allows them to virtually experience the world of the GDR. It was developed by students as part of a group project of the Chair of Computer Graphics and Visualization at the Technical University of Dresden.\nDevelopers:\nDavid Victor Raj Anthony\nJan Zimmermann? \nMasoud Taghikhah\nOlena Horokh\nPaul Hunt\nSneha Verma Prakash\nTania Krisanty\nTanzila Sadika\nVictor\nTechnologies used are Unity, MapBox, ARCore/ARFoundation." };

        private static readonly string[] clearDataTexts = { "Daten löschen", "Clear data" };

        // UI
        public Canvas canvas;
    
        public Text titleText;
        public Text captionText;

        public Text clearDataText;

        void Update()
        {
            // Make sure user is on Android platform
            if (Application.platform == RuntimePlatform.Android)
            { 
                // Check if Back was pressed this frame
                if (Input.GetKeyDown(KeyCode.Escape))
                    Hide();
            }
        }

        public void Show(int language)
        {
            Debug.Log("About::Show " + language);

            titleText.text = titles[language];
            captionText.text = captions[language];

            clearDataText.text = clearDataTexts[language];

            canvas.enabled = true;
        }

        public void Hide()
        {
            Debug.Log("About::Hide");

            canvas.enabled = false;
            gameObject.SetActive(false);
        }

        public void ClearCache()
        {
            Davinci.ClearAllCachedFiles();
        }
    }
}
