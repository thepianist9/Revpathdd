using System.Collections;
using System.Collections.Generic;
using HistocachingII;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class TagButtonItem : MonoBehaviour
    {
        [HideInInspector] public string tagName;
        [HideInInspector] public TagScrollViewController tagScrollViewController;
        [SerializeField] private Text tagButtonText;



        // Start is called before the first frame update
        void Start()
        {
            tagButtonText.text = tagName;
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        void OnClick()
        {
            Gallery.Instance.SetTagsList(tagName);
        }

    }
}