 using System;
 using System.Collections;
 using TMPro;
 using UnityEngine;
 using UnityEngine.EventSystems;
 using UnityEngine.Networking;
 using UnityEngine.UI;
 using UnityEngine.Video;

 namespace HistocachingII
 {
     public class FullScreen : MonoBehaviour
     {
         private static FullScreen _Instance;
         public static FullScreen Instance { get { return _Instance; } }

         [SerializeField] private Button fScrnBtn;
         [SerializeField] private Sprite fullScrnSprite;
         [SerializeField] private Sprite retrScrnSprite;
         [SerializeField] private GameObject player;
         [SerializeField] private Button playPauseButton;
         [SerializeField] private Sprite playSprite;
         [SerializeField] private Sprite pauseSprite;
         [SerializeField] private Button stopButton;
         [SerializeField] private TextMeshProUGUI playTimeText;

         [SerializeField] private Image progress;

         GameObject audioPlayer;
         private AudioSource _audioSource;
         GameObject videoPlayer;
         private VideoPlayer rawImage;
         public string media = null;

         private int fullLength;
         private int playTime;
         private int seconds;
         private int minutes;
         


         public bool _fullScreen = false;
        

         public void initPlayer(string url)
         {
             if (_Instance == null) _Instance = this;
       
             if (media == "Video")
             {
                 videoPlayer = player.transform.Find("VideoPlayer").gameObject;
                 videoPlayer.SetActive(true);
                 
                 rawImage = videoPlayer.GetComponentInChildren<VideoPlayer>();
                 rawImage.url = url;
                 rawImage.playOnAwake = false;
                 fScrnBtn.onClick.AddListener(EnterFullScrn);
                 
             }
             else if(media == "Audio")
             {
                 StartCoroutine(GetAudioClip(url));  
                 
             }
         }
         IEnumerator GetAudioClip(String url)
         {
             using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.OGGVORBIS))
             {
                 yield return www.SendWebRequest();

                 if (www.result == UnityWebRequest.Result.ConnectionError)
                 {
                     Debug.Log(www.error);
                 }
                 else
                 {
                     AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                     audioPlayer = player.transform.Find("AudioPlayer").gameObject;
                     audioPlayer.SetActive(true);
                     _audioSource = audioPlayer.GetComponent<AudioSource>();
                     _audioSource.clip = myClip;

                     fullLength = (int) myClip.length;
                     
                     
                     
                     playTimeText.text = minutes + ":" + seconds.ToString("D2") + "/" + ((fullLength / 60) % 60) + ":" + (fullLength % 60).ToString("D2");
                     progress = progress.GetComponent<Image>();
                     playPauseButton.onClick.AddListener(VidPlayPauseFunc);
                     stopButton.onClick.AddListener(VidStop);
                 }
             }
         }

        

        
         private void Update()
         {
             if (media == "Video")
             {
                 if (rawImage.frameCount > 0)
                 {
                     progress.fillAmount = (float) rawImage.frame / rawImage.frameCount;
                 }   
             }

             if (media == "Audio")
             {
                 if (_audioSource.isPlaying)
                 {
                     playTime = (int) _audioSource.time;
                     seconds = playTime % 60;
                     minutes = (playTime / 60) % 60;
                     playTimeText.text = minutes + ":" + seconds.ToString("D2") + "/" + ((fullLength / 60) % 60) + ":" + (fullLength % 60).ToString("D2");

                     progress.fillAmount = (float) playTime / fullLength;

                 }
             }
             
         }

         private void VidPlayPauseFunc()
         {
             if (media == "Audio")
             {
                 if (_audioSource.isPlaying)
                 {
                     _audioSource.Pause();
                     playPauseButton.GetComponent<Image>().sprite = pauseSprite;
                 }
                 else
                 {
                     _audioSource.Play();
                     playPauseButton.GetComponent<Image>().sprite = playSprite;
                 }
             }
             else if (media == "Video")
             {

                 if (rawImage.isPlaying)
                 {
                     rawImage.Pause();
                     playPauseButton.GetComponent<Image>().sprite = pauseSprite;

                 }
                 else
                 {
                     rawImage.Play();
                     playPauseButton.GetComponent<Image>().sprite = playSprite;
                
                 }
             }
         }

         public void VidStop()
         {
             if (media == "Video")
             {
                 progress.fillAmount = 0f;
                 playPauseButton.GetComponent<Image>().sprite = pauseSprite;
                 rawImage.Stop();
             }
             
             else if (media == "Audio")
             {
                 progress.fillAmount = 0f;
                 playPauseButton.GetComponent<Image>().sprite = pauseSprite;
                 rawImage.Stop();
             }
         }
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
             var frame = rawImage.frameCount * pct;
             rawImage.frame = (long)frame;
         }
         
         private void EnterFullScrn()
         {
             
         }
     }
     
 }