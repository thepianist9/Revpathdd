using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Events;
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

        private bool fullScreen = false;
        private GameObject img;

        //TODO: Media Controls

        private string url;

        Vector3 touchStart;
        public float min_text_size = 25;
        public float max_text_size = 100;


        // Update is called once per frame
        void Update () 
        {

            if(Input.GetKeyDown("i"))
            {
                descriptionText.fontSize+= 3;
                Debug.Log("i is being pressed");
            }
            if(Input.GetKeyDown("o"))
            {
                descriptionText.fontSize-= 3;
            }

       
            if(Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);
 
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
 
                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
 
                float difference = currentMagnitude - prevMagnitude;
 
                zoom(difference);

            }     
         
        }
 
        void zoom(float increment)
        {
            descriptionText.fontSize+= (int)increment;
        }


        public void ResetZoom()
        {
            descriptionText.fontSize = (int)min_text_size;
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

            Button fScrn = image.gameObject.AddComponent<Button>();
            fScrn.onClick.AddListener(FullScrn);
        }


        public void FullScrn()
        {
            img = GameObject.Find("DocumentsCanvas/Panel/Image");
            Image i = img.AddComponent<Image>();
            RectTransform parent = GameObject.Find("DocumentsCanvas/Panel").GetComponent<RectTransform>();
           
            
            img.GetComponent<RectTransform>().sizeDelta = new Vector2(parent.rect.height, parent.rect.width);
            img.SetActive(true);
            i.sprite = image.sprite;
            fullScreen = true;
            
            img.GetComponent<Button>().onClick.AddListener(ExitFScrn);

        }

        private void ExitFScrn()
        {
            if (fullScreen)
            {
                img.SetActive(false);
                Destroy(img.GetComponent<Image>());
                fullScreen = false;
            }
        }

    }

}
