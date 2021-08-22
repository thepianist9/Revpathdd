using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class DocumentUI : MonoBehaviour
    {
        // UI
        public Texture2D loading, error;
        
        public Image image;
        public AspectRatioFitter aspectRatioFitter;

        public Text captionText;
        public Text descriptionText;

        private string url;

        // Start is called before the first frame update
        void Start()
        {
        }

        void Destroy()
        {
        }

        public void SetText(string caption, string description)
        {
            captionText.text = caption;
            descriptionText.text = description;
        }

        public void SetPhotoURL(string url, float aspectRatio)
        {
            if (url.Equals(this.url))
                return;

            this.url = url;

            aspectRatioFitter.aspectRatio = 1f / aspectRatio;
            
            if (image.sprite != null)
            {
                Destroy(image.sprite.texture);
                Destroy(image.sprite);
            }

            Davinci.get()
                .load(url)
                .setLoadingPlaceholder(loading)
                .setErrorPlaceholder(error)
                .setFadeTime(0)
                .into(image)
                .start();
        }
    }
}
