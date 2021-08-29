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

        private string url;

        // Start is called before the first frame update
        void Start()
        {
        }

        void Destroy()
        {
        }

        // void Update()
        // {
        //     if (Input.touchSupported)
        //     {
        //         // Pinch to zoom
        //         if (Input.touchCount == 2)
        //         {
        //             // Utils.ForceCrash(ForcedCrashCategory.Abort);

        //             // get current touch positions
        //             Touch touchZero = Input.GetTouch(0);
        //             Touch touchOne = Input.GetTouch(1);

        //             // get touch position from the previous frame
        //             Vector2 touchZeroPreviousPos = touchZero.position - touchZero.deltaPosition;
        //             Vector2 touchOnePreviousPos = touchOne.position - touchOne.deltaPosition;

        //             float prevTouchDeltaMag = (touchZeroPreviousPos - touchOnePreviousPos).magnitude;
        //             float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        //             // float oldTouchDistance = Vector2.Distance (tZeroPrevious, tOnePrevious);
        //             // float currentTouchDistance = Vector2.Distance (tZero.position, tOne.position);

        //             float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

        //             Zoom(deltaMagnitudeDiff, 0.5f);

        //             // get offset value
        //             // float deltaDistance = oldTouchDistance - currentTouchDistance;
        //             // Zoom(deltaDistance, 0);

        //             // scaleChange = deltaMagnitudeDiff * zoomSpeedPinch;

        //             // midPoint = (touchOne.position + touchZero.position) / 2;
        //         }
        //     }
        // }

        private void Zoom(float distance, float speed)
        {
            Vector3 scale = image.transform.localScale + new Vector3(distance * speed, distance * speed, 1f);
            image.transform.localScale = Vector3.Max(scale, new Vector3(1f, 1f, 1f));
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
    }
}
