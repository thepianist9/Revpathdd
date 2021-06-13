using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class CategoryItem : MonoBehaviour
    {
        private Text text;

        // Start is called before the first frame update
        void Start()
        {
        }

        public void SetTitle(string title)
        {
            if (text == null)
                text = GetComponent<Text>();

            text.text = title;
        }
    }
}
