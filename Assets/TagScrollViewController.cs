using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace HistocachingII
{
   public class TagScrollViewController : MonoBehaviour
   {
       private static TagScrollViewController _Instance;
       public static TagScrollViewController Instance { get { return _Instance; } }
       
        [SerializeField] private GameObject tagBtnPref;
        [SerializeField] private Transform tagBtnParent;

        public int language;
        
        
        
        
        void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }
        }
        

        public void LoadTagButtons(List<Tag> tags, int language)
        {
            this.language = language;
            foreach (Tag tag in tags)
            {
                GameObject tagBtnObj = Instantiate(tagBtnPref, tagBtnParent);
                TagButtonItem tagButtonItem = tagBtnObj.GetComponent<TagButtonItem>();
                tagButtonItem.tag = tag;
                tagButtonItem.language = language;
                tagButtonItem.tagScrollViewController = this;
            }
        }

        public void ClearTags()
        {
            foreach (Transform tag in tagBtnParent.transform)
            {
                tag.gameObject.GetComponent<Image>().color = Color.white;
            }
        }

        public void DestroyTags()
        {
            foreach (Transform tag in tagBtnParent.transform)
            {
                Destroy(tag.gameObject);
            }
        }
    }
}
