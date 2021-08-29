using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class FilterItem : MonoBehaviour
    {
        public Toggle toggle;

        public Text nameText;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        public void SetName(string name)
        {
            nameText.text = name;
        }

        public void SetSelected(bool selected)
        {
            toggle.isOn = selected;
        }
    }
}
