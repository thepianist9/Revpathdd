
using System.IO;
using UnityEngine;

using UnityEngine.UI;

namespace HistocachingII
{
    public class TourItem : RecyclingListViewItem
    {
        public Texture2D loading, error;

        public Image image;

        public Text titleText;
        public Text subtitleText;

        public GameObject AR;

        private string url;

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

            if (image.sprite != null)
            {
                DestroyImmediate(image.sprite.texture, true);
                DestroyImmediate(image.sprite, true);
            }

            // Do not use Path.GetDirectoryName and Path.Combine as they will strip the double slash in https into single slash
            string extension = Path.GetExtension(url);
            string file = url.Substring(0, url.Length - extension.Length);

            string thumbnail = file + "-thumb" + extension;

            Davinci.get()
                .load(thumbnail)
                // .setLoadingPlaceholder(loading)
                // .setErrorPlaceholder(error)
                .setFadeTime(0)
                .into(image)
                .withLoadedAction(() =>
                {
                    Debug.Log("HistocacheItem::SetPhotoURL loaded " + thumbnail);

                    // This is a hack(?) to achieve both cover (aspect fill) on image (child) & rounded corner mask on button (parent),
                    // without this the scale type of the image is aspect fit because mask forces its child to resize (I guess).
                    float scale = AspectRatioFitter.aspectRatio / aspectRatio;                    
                    
                    image.transform.localScale = scale > 1f ? new Vector3(1f, scale, 1f) : new Vector3(1f / scale, 1f, 1f);
                })
                .start();
        }

        public void SetAR(bool available)
        {
            AR.SetActive(available);
        }
    }
}
