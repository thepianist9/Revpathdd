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
        [HideInInspector] public Tag tag;
        [HideInInspector] public TagScrollViewController tagScrollViewController;
        [SerializeField] private Text tagButtonText;

        private Image img;
        public int language;

        // Start is called before the first frame update
        void Start()
        {
            tagButtonText.text = (language == 0) ? tag.title_de : tag.title_en;
            img = GetComponent<Image>();
            GetComponent<Button>().onClick.AddListener(OnClick);
            
        }

        void OnClick()
        {
            Color btnColor = img.color;
            if (btnColor == Color.cyan)
            {
                Gallery.Instance.SetTagsList(tag, "remove");
                img.color = Color.white;
            }
            else
            {
                Gallery.Instance.SetTagsList(tag, "add");
                img.color = Color.cyan;
            }


        }

    }
}