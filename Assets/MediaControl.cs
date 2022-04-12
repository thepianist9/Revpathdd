using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MediaControl : MonoBehaviour
{

    [SerializeField] private Toggle mediaToggle;

    [SerializeField] private RawImage videoTextureImage;

    private VideoPlayer _videoPlayer = null;
    
    // Start is called before the first frame update
    void Start()
    { 
        _videoPlayer = videoTextureImage.GetComponent<VideoPlayer>();
        mediaToggle.onValueChanged.AddListener(OnToggle);
    }

    void OnToggle(bool flag)
    {
        if (flag == true) _videoPlayer.Play();
        if (flag == true) _videoPlayer.Play();
        else _videoPlayer.Pause();
       
    }

}
