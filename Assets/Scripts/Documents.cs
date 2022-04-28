using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;

namespace HistocachingII
{
    public class Documents : MonoBehaviour
    {
        public ScreenManager screenManager;

        // UI
        public Canvas canvas;

        // App bar (left-to-right order)        
        public Text titleText;
        public Button linkButton;
        public Button ARButton;
        public GameObject filler;    // this is so that the title text will still be centered when link and AR buttons are not visible

        public ScrollRect scrollRect;
        public RectTransform content;

        // Language
        private int language;

        // Game Object
        public GameObject documentItemTemplate;

        private List<DocumentItem> documentItems = new List<DocumentItem>();

#if UNITY_ANDROID
        void Update()
        {
            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
                Hide();
        }
#endif

        public void Show(int language, Histocache histocache)
        {
            Debug.Log("Documents::Show " + language);

            this.language = language;

            canvas.enabled = true;

            SetTitle(language == 0 ? histocache.title_de : histocache.title_en);

            bool needFiller = true;

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
                    linkButton.onClick.AddListener(() =>
                    {
                        // Currently any parameter will open the German language page, but here we use "?lang=de" to make it neat
                        Application.OpenURL(language == 0 ? histocache.add_info_url + "?lang=de" : histocache.add_info_url);
                    });

                    linkButton.gameObject.SetActive(true);

                    needFiller = false;
                }
                else
                {
                    linkButton.gameObject.SetActive(false);
                }
            }

            if (histocache.is_displayed_on_table)
            {
                if (ARSession.state == ARSessionState.Ready)
                {
                    ARButton.onClick.RemoveAllListeners();
                    ARButton.onClick.AddListener(() => screenManager.SwitchToCameraScreen(histocache));
    
                    ARButton.gameObject.SetActive(true);

                    needFiller = false;
                }
                else
                {
                    ARButton.gameObject.SetActive(false);
                }
            }
            else
            {
                ARButton.gameObject.SetActive(false);
            }

            filler.SetActive(needFiller);

            StartCoroutine(SetDocuments(histocache));
        }

        public void Hide()
        {
            Debug.Log("Documents::Hide");
            // if (inst._fullScreen)
            // {
            //     inst.FSCR();
            // }
            canvas.enabled = false;
            gameObject.SetActive(false);
            
            foreach (DocumentItem item in documentItems)
            {
                item.gameObject.SetActive(false);
            }

            scrollRect.normalizedPosition = new Vector2(0, 1);
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

            SetDocument(index++, language == 0 ? histocache.caption_de : histocache.caption_en, language == 0 ? histocache.description_de : histocache.description_en, histocache.file_url, histocache.image_aspect_ratio);

            if (histocache.documents.Length > 0)
            {
                foreach (Document document in histocache.documents)
                {
                    yield return null;

                    SetDocument(index++, language == 0 ? document.caption_de : document.caption_en, language == 0 ? document.description_de : document.description_en, document.file_url,  document.media_url, document.image_aspect_ratio); 
                }
            }
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

                item = gameObject.GetComponentInChildren<DocumentItem>();
                documentItems.Add(item);


                item.SetPhotoURL(url, aspectRatio);
                item.SetText(caption, description);
                item.gameObject.SetActive(true);

            }
        }

 
        private void SetDocument(int index, string caption, string description, string url, string media_url, float aspectRatio)
        {
            DocumentItem item;
            
            GameObject gameObject = Instantiate(documentItemTemplate);
            gameObject.transform.SetParent(content, false);
            gameObject.transform.localScale = Vector3.one;

            item = gameObject.GetComponentInChildren<DocumentItem>();
            documentItems.Add(item);
            
            
            
            string extension = null;
            
            if (media_url != null)
            {
                extension = Path.GetExtension(media_url);
                string file = url.Substring(0, url.Length - extension.Length);

            }
            

            FullScreen instance = gameObject.GetComponent<FullScreen>();
            item.SetPhotoURL(url, aspectRatio);
            item.SetText(caption, description);
            item.gameObject.SetActive(true);

            if (extension == ".mp4")
            {
                instance.media = "Video";
                instance.initPlayer(media_url);

            }
            else if (extension == ".mp3")
            {
                //add audio url
                instance.media = "Audio";
                Debug.Log($"Audio document: {media_url}");
                instance.initPlayer(media_url);
            } 

                
        }
        
    }
}
