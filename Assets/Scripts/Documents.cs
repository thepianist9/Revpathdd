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
        public Button linkButton;
        public Text titleText;

        public RectTransform content;

        // Language
        private int language;

        // Game Object
        public GameObject documentItemTemplate;

        private List<DocumentItem> documentItems = new List<DocumentItem>();

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

            content.gameObject.SetActive(true);

            SetTitle(language == 0 ? histocache.title_de : histocache.title_en);

            if (string.IsNullOrWhiteSpace(histocache.add_info_url))
            {
                linkButton.gameObject.SetActive(false);
            }
            else
            {
                Uri uri;
                if (Uri.TryCreate(histocache.add_info_url, UriKind.Absolute, out uri) && uri.Scheme == Uri.UriSchemeHttps)
                {
                    linkButton.onClick.RemoveAllListeners();
                    linkButton.onClick.AddListener(() => Application.OpenURL(histocache.add_info_url));

                    linkButton.gameObject.SetActive(true);
                }
                else
                {
                    linkButton.gameObject.SetActive(false);
                }
            }

            StartCoroutine(SetDocuments(histocache));
        }

        public void Hide()
        {
            Debug.Log("Documents::Hide");

            canvas.enabled = false;

            content.gameObject.SetActive(false);

            linkButton.gameObject.SetActive(false);

            foreach (DocumentItem item in documentItems)
            {
                item.gameObject.SetActive(false);
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

        private IEnumerator SetDocuments(Histocache histocache)
        {
            yield return null;

            int index = 0;

            SetDocument(index++, language == 0 ? histocache.caption_de : histocache.caption_en, language == 0 ? histocache.description_de : histocache.description_en, histocache.image_url, histocache.image_aspect_ratio);

            if (histocache.documents.Length > 0)
            {
                foreach (Document document in histocache.documents)
                {
                    yield return null;

                    SetDocument(index++, language == 0 ? document.caption_de : document.caption_en, language == 0 ? document.description_de : document.description_en, document.image_url, document.image_aspect_ratio); 
                }
            }

            // LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }

        private void SetDocument(int index, string caption, string description, string url, float aspectRatio)
        {
            DocumentItem item;

            if (documentItems.Count > index)
            {
                item = documentItems[index];
            }
            else
            {
                GameObject gameObject = Instantiate(documentItemTemplate);
                gameObject.transform.SetParent(content, false);
                gameObject.transform.localScale = Vector3.one;

                item = gameObject.GetComponent<DocumentItem>();

                documentItems.Add(item);
            }

            item.SetPhotoURL(url, aspectRatio);
            item.SetText(caption, description);

            item.gameObject.SetActive(true);
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
