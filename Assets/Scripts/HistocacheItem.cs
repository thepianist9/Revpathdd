using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace HistocachingII
{
    public class HistocacheItem : RecyclingListViewItem
    {
        public Texture2D loading, error;

        public Image image;

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

            // Do not use Path.GetDirectoryName and Path.Combine as they will strip the double slash in https into single slash
            string extension = Path.GetExtension(url);
            string file = url.Substring(0, url.Length - extension.Length);

            string thumbnail = file + "-thumb" + extension;

            if (image.sprite != null)
            {
                Destroy(image.sprite.texture);
                Destroy(image.sprite);
            }

            Davinci.get()
                .load(thumbnail)
                // .setLoadingPlaceholder(loading)
                // .setErrorPlaceholder(error)
                .setFadeTime(0)
                .into(image)
                .withLoadedAction(() =>
                {
                    // This is a hack(?) to achieve both cover (aspect fill) on image (child) & rounded corner mask on button (parent),
                    // without this the scale type of the image is aspect fit because mask forces its child to resize (I guess).
                    float scale = AspectRatioFitter.aspectRatio * aspectRatio;                    
                    
                    image.transform.localScale = scale > 1f ? new Vector3(1f, scale, 1f) : new Vector3(1f / scale, 1f, 1f);
                })
                .start();
        }
    }
}
