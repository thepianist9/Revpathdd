using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HistocachingII
{
   public class TagScrollViewController : MonoBehaviour
   {
       private static TagScrollViewController _Instance;
       public static TagScrollViewController Instance { get { return _Instance; } }
       
        [SerializeField] private GameObject tagBtnPref;
        [SerializeField] private Transform tagBtnParent;
        
        
        
        
        void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }
        }
        

        public void LoadTagButtons(List<Tag> tags)
        {
            foreach (Tag tag in tags)
            {
                GameObject tagBtnObj = Instantiate(tagBtnPref, tagBtnParent);
                tagBtnObj.GetComponent<TagButtonItem>().tagName = tag.title_en;
                tagBtnObj.GetComponent<TagButtonItem>().tagScrollViewController = this;
            }
        }
    }
}
