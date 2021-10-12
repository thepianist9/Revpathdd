using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class FilterItem : MonoBehaviour
    {
        private static readonly Color selectedColor = new Color(17/255f, 15/255f, 14/255f);
        private static readonly Color unselectedColor = new Color(94/255f, 102/255f, 106/255f);

        public Toggle toggle;

        public Text nameText;

        // Start is called before the first frame update
        void Start()
        {
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        void Destroy()
        {
            toggle.onValueChanged.RemoveListener(OnToggleChanged);
        }

        private void OnToggleChanged(bool selected)
        {
            nameText.color = selected ? selectedColor : unselectedColor;
        }

        public void SetName(string name)
        {
            nameText.text = name;
        }

        public void SetUnselected(bool unselected)
        {
            toggle.isOn = !unselected;
        }

        public bool IsUnselected()
        {
            return !toggle.isOn;
        }
    }
}
