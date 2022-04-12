 using System;
 using Mapbox.Unity.Utilities;
 using UnityEngine;
 using UnityEngine.EventSystems;
 using UnityEngine.UI;
 using UnityEngine.Video;

 namespace HistocachingII
 {
     public class FullScreen : MonoBehaviour
     {
         private static FullScreen _Instance;
         public static FullScreen Instance { get { return _Instance; } }

         [SerializeField] private Button fScrnBtn;
         [SerializeField] private RawImage videoTexture;
         [SerializeField] private Canvas _canvas;
         [SerializeField] private Sprite fullScrnSprite;
         [SerializeField] private Sprite retrScrnSprite;
         [SerializeField] private GameObject mediaPlayer;
         [SerializeField] private Button playPauseButton;
         [SerializeField] private Sprite playSprite;
         [SerializeField] private Sprite pauseSprite;
         [SerializeField] private Button stopButton;
         public Texture2D audioTexture;
         
         [SerializeField] private Image progress;

         AudioSource audioPlayer;
         VideoPlayer videoPlayer;
         public string media = null;


         public bool _fullScreen = false;
         RawImageInfo originalImgInfo;

         void Awake()
         {
             if (_Instance == null) _Instance = this;
       
             if (media == "Video")
             {
                 videoPlayer = mediaPlayer.GetComponentInChildren<VideoPlayer>();
                 
                 fScrnBtn.onClick.AddListener(FSCR);
                 originalImgInfo = GetImageSettings(videoTexture);
                 progress = progress.GetComponent<Image>();
                 playPauseButton.onClick.AddListener(VidPlayPauseFunc);
                 stopButton.onClick.AddListener(VidStop);
             }
             else if(media == "Audio")
             {
                 audioPlayer = mediaPlayer.GetComponentInChildren<AudioSource>();
                 videoTexture.texture = audioTexture;
                 progress = progress.GetComponent<Image>();
                 playPauseButton.onClick.AddListener(VidPlayPauseFunc);
                 stopButton.onClick.AddListener(VidStop);
             }
             

         }

         public struct RawImageInfo
         {
             public Vector3 anchorPos;
             public Vector2 widthAndHeight;
             public Vector2 anchorMin;
             public Vector2 anchorMax;
             public Vector2 pivot;

             public Vector2 offsetMin;
             public Vector2 offsetMax;

             public Quaternion rot;
             public Vector3 scale;

         }

         public void FSCR()
         {
             if (!_fullScreen)
             {
                 // videoPlayer.GetComponent<LayoutElement>().ignoreLayout = true;

                 RectTransform rectTransform = videoTexture.rectTransform;

                 Vector3 rot = videoTexture.rectTransform.rotation.eulerAngles;
                 rot.z = -90;
                 rectTransform.rotation = Quaternion.Euler(rot);
                 RectTransform canvasRectTransform = GameObject.FindWithTag("doc").GetComponent<RectTransform>();
                 // RectTransform canvasRectTransform = _canvas.GetComponent<RectTransform>();

                 // rectTransform.sizeDelta = new Vector2(canvasRectTransform.rect.height, canvasRectTransform.rect.width);

                 float aspRatio = canvasRectTransform.rect.size.x / canvasRectTransform.rect.size.y;
                 float halfAspRatio = aspRatio / 2.0f;
                 float halfAspRatioInvert = (1.0f / aspRatio) / 2.0f;


                 rectTransform.anchorMin = new Vector2(0.5f - halfAspRatioInvert, 0.5f - halfAspRatio);
                 rectTransform.anchorMax = new Vector2(0.5f + halfAspRatioInvert, 0.5f + halfAspRatio);
                 rectTransform.anchoredPosition3D = Vector3.zero;
                 rectTransform.pivot = new Vector2(0.5f, 0.5f);
                 rectTransform.offsetMin = Vector2.zero;
                 rectTransform.offsetMax = Vector2.zero;
                 fScrnBtn.GetComponent<Image>().sprite = retrScrnSprite;

                 _fullScreen = true;

             }
             else
             {
                 ApplyImageSettings(videoTexture, originalImgInfo);
                 videoTexture.GetComponent<LayoutElement>().ignoreLayout = false;
                 fScrnBtn.GetComponent<Image>().sprite = fullScrnSprite;
                 _fullScreen = false;
             }
         }

         RawImageInfo GetImageSettings(RawImage rawImage)
         {
             RectTransform rectTrfm = rawImage.rectTransform;

             RawImageInfo rawImgInfo = new RawImageInfo();

             //Get settings from RawImage and store as RawImageInfo 
             rawImgInfo.anchorPos = rectTrfm.anchoredPosition3D;
             rawImgInfo.widthAndHeight = rectTrfm.sizeDelta;

             rawImgInfo.anchorMin = rectTrfm.anchorMin;
             rawImgInfo.anchorMax = rectTrfm.anchorMax;
             rawImgInfo.pivot = rectTrfm.pivot;

             rawImgInfo.offsetMin = rectTrfm.offsetMin;
             rawImgInfo.offsetMax = rectTrfm.offsetMax;

             rawImgInfo.rot = rectTrfm.rotation;
             rawImgInfo.scale = rectTrfm.localScale;

             return rawImgInfo;
         }

         private void ApplyImageSettings(RawImage rawImage, RawImageInfo rawImgInfo)
         {
             RectTransform rectTrfm = rawImage.rectTransform;

             //Apply settings from RawImageInfo to RawImage RectTransform
             rectTrfm.anchoredPosition3D = rawImgInfo.anchorPos;
             rectTrfm.sizeDelta = rawImgInfo.widthAndHeight;

             rectTrfm.anchorMin = rawImgInfo.anchorMin;
             rectTrfm.anchorMax = rawImgInfo.anchorMax;
             rectTrfm.pivot = rawImgInfo.pivot;

             rectTrfm.offsetMin = rawImgInfo.offsetMin;
             rectTrfm.offsetMax = rawImgInfo.offsetMax;

             rectTrfm.rotation = rawImgInfo.rot;
             rectTrfm.localScale = rawImgInfo.scale;
         }
         private void Update()
         {
             if (media == "Video")
             {
                 if (videoPlayer.frameCount > 0)
                 {
                     progress.fillAmount = (float) videoPlayer.frame / videoPlayer.frameCount;
                 }   
             }
             
         }

         private void VidPlayPauseFunc()
         {
             if (media == "Audio")
             {
                 if (audioPlayer.isPlaying)
                 {
                     audioPlayer.Pause();
                     playPauseButton.GetComponent<Image>().sprite = pauseSprite;
                 }
                 else
                 {
                     audioPlayer.Play();
                     playPauseButton.GetComponent<Image>().sprite = playSprite;
                 }
             }
             else if (media == "Video")
             {

                 if (videoPlayer.isPlaying)
                 {
                     videoPlayer.Pause();
                     playPauseButton.GetComponent<Image>().sprite = pauseSprite;

                 }
                 else
                 {
                     videoPlayer.Play();
                     playPauseButton.GetComponent<Image>().sprite = playSprite;
                
                 }
             }
         }

         public void VidStop()
         {
             if (videoPlayer)
             {
                 progress.fillAmount = 0f;
                 playPauseButton.GetComponent<Image>().sprite = pauseSprite;
                 videoPlayer.Stop();
             }
         }

         // private void EnterFullScrn()
         // {
         //     
         // }

         public void OnDrag(PointerEventData eventData)
         {
             TrySkip(eventData);
         }
         public void OnPointerDown(PointerEventData eventData)
         {
             TrySkip(eventData);
         }

         private void TrySkip(PointerEventData eventData)
         {
             Vector2 localPoint;
             if (RectTransformUtility.ScreenPointToLocalPointInRectangle(progress.rectTransform, eventData.position, null,
                     out localPoint))
             {
                 float pct = Mathf.InverseLerp(progress.rectTransform.rect.xMin, progress.rectTransform.rect.xMax,
                     localPoint.x);
                 SkipToPercent(pct);
             }
         }

         private void SkipToPercent(float pct)
         {
             var frame = videoPlayer.frameCount * pct;
             videoPlayer.frame = (long)frame;
         }
     }
     
 }