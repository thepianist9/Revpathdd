using UnityEngine.UI;

namespace HistocachingII
{
    public class CategoryItem : RecyclingListViewItem
    {
        private Text text;

        public void SetTitle(string title)
        {
            if (text == null)
                text = GetComponent<Text>();

            text.text = title;
        }
    }
}
