using UnityEngine;
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
        private float aspectRatio;

        private bool fullScreen = false;
        private GameObject img;

        //TODO: Media Controls

        private string url;
        private GameObject Overlay;

        Vector3 touchStart;
        public float min_text_size = 25;
        public float max_text_size = 80;


        // Update is called once per frame
        void Update () 
        {

            if(Input.GetKeyDown("i"))
            {
                zoom(3);
                Debug.Log("i is being pressed");
            }
            if(Input.GetKeyDown("o"))
            {
                zoom(-3);
            }

       
            if(Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);
 
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
 
                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
 
                float difference = (currentMagnitude - prevMagnitude)*0.1f;
 
                zoom(difference);

            }     
         
        }
 
        void zoom(float increment)
        {
            int size = descriptionText.fontSize + (int)increment;

            if (size >= min_text_size && size <= max_text_size)
            {
                descriptionText.fontSize = size;
            }
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
            this.aspectRatio = aspectRatio;
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
            Overlay = GameObject.Find("Overlay").gameObject;
            Overlay.GetComponent<Canvas>().enabled = true;
            GameObject img = Overlay.transform.Find("FscrnImg").gameObject;
            
            Image i = img.GetComponent<Image>();
            
            RectTransform parent = Overlay.GetComponent<RectTransform>();
            img.GetComponent<RectTransform>().sizeDelta = new Vector2(parent.rect.height, parent.rect.width);
            img.GetComponent<AspectRatioFitter>().aspectRatio = 1;
            
            i.sprite = image.sprite;
            Overlay.SetActive(true);
            fullScreen = true;

            img.GetComponent<Button>().onClick.AddListener(ExitFScrn);

        }

        private void ExitFScrn()
        {
            if (fullScreen)
            {
                Overlay.GetComponent<Canvas>().enabled = false;
                fullScreen = false;
            }
        }

    }

}
