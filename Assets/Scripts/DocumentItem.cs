using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

namespace HistocachingII
{
    public class DocumentItem : MonoBehaviour
    {
        // UI
        public Texture2D loading, error;
        
        public Image image;
        public AspectRatioFitter aspectRatioFitter;

        public Text descriptionText;
        public Text captionText;
        [SerializeField] private RenderTexture _texture;
        
        //TODO: Media Controls
        
        private string url;

        // void Update()
        // {
        //     // Pinch to zoom
        //     if (Input.touchCount == 2)
        //     {
        //         float currentTouchDistance = getTouchDistance();
        //         float deltaTouchDistance = currentTouchDistance - touchDistanceOrigin;
        //         float scalePercentage = (deltaTouchDistance / 1200f) + 1f;

        //         Vector3 scaleTemp = transform.localScale;
        //         scaleTemp.x = scalePercentage * originalScale.x;
        //         scaleTemp.y = scalePercentage * originalScale.y;
        //         scaleTemp.z = scalePercentage * originalScale.z;

        //         //to make the object snap to 100% a check is being done to see if the object scale is close to 100%,
        //         //if it is the scale will be put back to 100% so it snaps to the normal scale.
        //         //this is a quality of life feature, so its easy to get the original size of the object.
        //         if (scaleTemp.x * 100 < 102 && scaleTemp.x * 100 > 98)
        //         {
        //             scaleTemp.x = 1;
        //             scaleTemp.y = 1;
        //             scaleTemp.z = 1;
        //         }
        //         //here we apply the calculation done above to actually make the object bigger/smaller.
        //         transform.localScale = scaleTemp;
        //     }
        // }

        // private void Zoom(float distance, float speed)
        // {
        //     Vector3 scale = image.transform.localScale + new Vector3(distance * speed, distance * speed, 1f);
        //     image.transform.localScale = Vector3.Max(scale, new Vector3(1f, 1f, 1f));
        // }

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

            aspectRatioFitter.aspectRatio = aspectRatio;
            
            if (image.sprite != null)
            {
                DestroyImmediate(image.sprite.texture, true);
                DestroyImmediate(image.sprite, true);
            }

            Davinci.get()
                .load(url)
                .setLoadingPlaceholder(loading)
                .setErrorPlaceholder(error)
                .setFadeTime(0)
                .into(image)
                .start();
        }

                public void ShowImage()
        {   
            image.gameObject.SetActive(true); 

        }

    }
}
