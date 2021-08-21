using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HistocachingII
{
    public class Documents : MonoBehaviour
    {
        // UI
        public Canvas canvas;
        
        public Button backButton;
        public Text titleText;

        // public ScrollRect scrollRect;
        public RectTransform content;

        // Language
        private int language;

        // Game Object
        public GameObject documentTemplate;

        private List<DocumentUI> documents = new List<DocumentUI>();

        // Start is called before the first frame update
        void Start()
        {
            backButton.onClick.AddListener(Hide);
        }

        void Destroy()
        {
            backButton.onClick.RemoveListener(Hide);
        }

        public void Show(int language, Histocache histocache)
        {
            Debug.Log("Documents::Show " + language);

            this.language = language;

            canvas.enabled = true;

            SetTitle(language == 0 ? histocache.title_de : histocache.title_en);

            int index = 0;

            SetDocument(index++, language == 0 ? histocache.caption_de : histocache.caption_en, language == 0 ? histocache.description_de : histocache.description_en, histocache.image_url, histocache.image_aspect_ratio);
            
            if (histocache.documents.Length > 0)
            {
                foreach (Document document in histocache.documents)
                {
                    SetDocument(index++, language == 0 ? document.caption_de : document.caption_en, language == 0 ? document.description_de : document.description_en, document.image_url, document.image_aspect_ratio);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }

        public void Hide()
        {
            Debug.Log("Documents::Hide");

            canvas.enabled = false;

            foreach (DocumentUI document in documents)
            {
                document.gameObject.SetActive(false);
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

        void SetDocument(int index, string caption, string description, string url, float aspectRatio)
        {
            DocumentUI document;

            if (documents.Count > index)
            {
                document = documents[index];
            }
            else
            {
                GameObject gameObject = Instantiate(documentTemplate);
                gameObject.transform.SetParent(content, false);
                gameObject.transform.localScale = Vector3.one;

                document = gameObject.GetComponent<DocumentUI>();

                documents.Add(document);
            }

            document.SetPhotoURL(url, aspectRatio);
            document.SetText(caption, description);

            document.gameObject.SetActive(true);
        }

        // public void OnDrag(PointerEventData eventData)
        // {
        //     Debug.Log("OK DRag");
        //     // Zoom();
        //     if (Input.touchCount <= 1) scrollRect.OnDrag(eventData);
        // }

        // public void OnBeginDrag(PointerEventData eventData)
        // {
        //     if (Input.touchCount <= 1) scrollRect.OnBeginDrag(eventData);
        // }

        // public void OnEndDrag(PointerEventData eventData)
        // {
        //     scrollRect.OnEndDrag(eventData);
        // }
    }
}
