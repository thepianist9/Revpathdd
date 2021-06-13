using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class POIItem : MonoBehaviour
    {
        public Texture2D loading, error;

        public Image image;
        public AspectRatioFitter aspectRatioFitter;

        public Text titleText;
        public Text subtitleText;

        private string url;

        // Start is called before the first frame update
        void Start()
        {
        }

        public void SetTitle(string title)
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
        }

        public void SetPhotoURL(string url, float aspectRatio)
        {
            if (url.Equals(this.url))
                return;

            this.url = url;
            
            Davinci.get()
                .load(url)
                // .setLoadingPlaceholder(loading)
                // .setErrorPlaceholder(error)
                .into(image)
                .setFadeTime(0)
                .withDownloadedAction(() =>
                {
                    // This is a hack(?) to achieve both cover (aspect fill) on image (child) & rounded corner mask on button (parent),
                    // without this the scale type of the image is aspect fit because mask forces its child to resize (I guess).
                    float scale = aspectRatioFitter.aspectRatio * aspectRatio;
                    
                    image.transform.localScale = scale > 1f ? new Vector3(1f, scale, 1f) : new Vector3(1f / scale, 1f, 1f);
                })
                .start();
        }
    }
}
