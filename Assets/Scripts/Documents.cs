using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class Documents : MonoBehaviour
    {
        // UI
        public Canvas canvas;
        
        public Button backButton;
        public Text titleText;

        public RectTransform content;

        // Language
        private int language;

        // Game Object
        public GameObject documentTemplate;

        private List<GameObject> documents = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            backButton.onClick.AddListener(Hide);
        }

        void Destroy()
        {
            backButton.onClick.RemoveListener(Hide);
        }

        public void Show(int language, POI poi)
        {
            Debug.Log("Documents::Show " + language);

            this.language = language;

            canvas.enabled = true;

            SetTitle(language == 0 ? poi.title_de : poi.title_en);

            int index = 0;

            SetPOI(index++, language == 0 ? poi.caption_de : poi.caption_en, language == 0 ? poi.description_de : poi.description_en, poi.image_url, poi.image_aspect_ratio);
            
            for (int i = 0; i < poi.documents?.Length; ++i)
            {
                POISubdocument document = poi.documents[i];

                SetPOI(index++, language == 0 ? document.caption_de : document.caption_en, language == 0 ? document.description_de : document.description_en, document.image_url, document.image_aspect_ratio);
            }
        }

        public void Hide()
        {
            Debug.Log("Documents::Hide");

            canvas.enabled = false;

            for (int i = 0; i < documents.Count; ++i)
            {
                GameObject gameObject = documents[i];
                gameObject.SetActive(false);
            }
        }

        public void SetTitle(string title)
        {
            string[] texts = title.Split('(');

            if (texts.Length > 0)
            {
                titleText.text = texts[0] + (texts.Length > 1 ? "\n(" + texts[1] : "");
            }
            else
            {
                titleText.text = "";
            }
        }

        void SetPOI(int index, string caption, string description, string url, float aspectRatio)
        {
            GameObject gameObject;

            if (documents.Count > index)
            {
                gameObject = documents[index];
            }
            else
            {
                gameObject = Instantiate(documentTemplate);
                gameObject.transform.SetParent(content, false);
                gameObject.transform.localScale = Vector3.one;

                documents.Add(gameObject);
            }

            POIDetail detail = gameObject.GetComponent<POIDetail>();
            detail.SetPhotoURL(url, aspectRatio);
            detail.SetText(caption, description);

            gameObject.SetActive(true);
        }
    }
}
