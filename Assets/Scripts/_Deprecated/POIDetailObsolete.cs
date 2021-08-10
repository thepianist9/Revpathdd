using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class POIDetailObsolete : MonoBehaviour
    {
        // UI
        public Texture2D loading, error;

        public Canvas canvas;
        
        // Image
        public Image image;
        public AspectRatioFitter aspectRatioFitter;

        public Button closeButton;

        // Drawer
        public Button previousButton;
        public Button nextButton;
        public Text titleText;
        public Text subtitleText;
        public Text descriptionText;
        public Text captionText;

        private string url;

        // Language
        private int language;

        // POI
        private POI poi;

        // POI index
        private int index;
        private int count;

        // Start is called before the first frame update
        void Start()
        {
            closeButton.onClick.AddListener(Hide);
            previousButton.onClick.AddListener(OnPrevious);
            nextButton.onClick.AddListener(OnNext);
        }

        void Destroy()
        {
            closeButton.onClick.RemoveListener(Hide);
            previousButton.onClick.RemoveListener(OnPrevious);
            nextButton.onClick.RemoveListener(OnNext);
        }

        public void Show(int language, POI poi)
        {
            Debug.Log("POIDetail::Show " + language);

            this.language = language;
            this.poi = poi;

            this.index = 0;
            this.count = poi.documents != null ? poi.documents.Length : 0;

            Debug.Log("POIDetail::Count " + count);

            if (language == 0)
                SetText(poi.caption_de, poi.title_de, poi.description_de);
            else
                SetText(poi.caption_en, poi.title_en, poi.description_en);

            SetPhotoURL(poi.image_url, poi.image_aspect_ratio);

            canvas.enabled = true;
        }

        public void Hide()
        {
            Debug.Log("POIDetail::Hide");

            canvas.enabled = false;
        }

        void OnPrevious()
        {
            index = (index + count) & count;

            Debug.Log("POIDetail::OnPrevious " + index);

            if (index == 0)
            {
                if (language == 0)
                    SetText(poi.caption_de, poi.title_de, poi.description_de);
                else
                    SetText(poi.caption_en, poi.title_en, poi.description_en);

                SetPhotoURL(poi.image_url, poi.image_aspect_ratio);
            }
            else
            {
                POISubdocument subdocument = poi.documents[index - 1];

                if (language == 0)
                    SetText(subdocument.caption_de, poi.title_de, subdocument.description_de);
                else
                    SetText(subdocument.caption_en, poi.title_en, subdocument.description_en);
                
                SetPhotoURL(subdocument.image_url, subdocument.image_aspect_ratio);
            }
        }

        void OnNext()
        {
            index = (index + 1) & count;

            Debug.Log("POIDetail::OnNext " + index);

            if (index == 0)
            {
                if (language == 0)
                    SetText(poi.caption_de, poi.title_de, poi.description_de);
                else
                    SetText(poi.caption_en, poi.title_en, poi.description_en);

                SetPhotoURL(poi.image_url, poi.image_aspect_ratio);
            }
            else
            {
                POISubdocument subdocument = poi.documents[index - 1];

                if (language == 0)
                    SetText(subdocument.caption_de, poi.title_de, subdocument.description_de);
                else
                    SetText(subdocument.caption_en, poi.title_en, subdocument.description_en);
                
                SetPhotoURL(subdocument.image_url, subdocument.image_aspect_ratio);
            }
        }

        void SetText(string caption, string title, string description)
        {
            string[] texts = title.Split('(');

            if (texts.Length > 0)
            {
                titleText.text = texts[0];

                subtitleText.text = texts.Length > 1 ? "(" + texts[1] : "";
            }
            else
            {
                titleText.text = "";
                subtitleText.text = "";
            }

            descriptionText.text = "\n" + description;

            captionText.text = "\n\n" + caption;
        }

        void SetPhotoURL(string url, float aspectRatio)
        {
            if (url.Equals(this.url))
                return;

            this.url = url;
            
            Davinci.get()
                .load(url)
                .setLoadingPlaceholder(loading)
                .setErrorPlaceholder(error)
                .setFadeTime(0)
                .into(image)
                // .withDownloadedAction(() =>
                // {
                //     // This is a hack(?) to achieve cover (aspect fill) on landscape image and
                //     // aspect fit on portrait image.
                //     float scale = aspectRatioFitter.aspectRatio * aspectRatio;
                    
                //     image.transform.localScale = new Vector3(1f / scale, 1f, 1f);
                // })
                .start();
        }
    }
}
