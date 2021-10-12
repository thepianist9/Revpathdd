using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HistocachingII
{
    [System.Serializable]
    public class FilterChangedEvent : UnityEvent<List<int>>
    {

    }

    public class CategoryFilter : MonoBehaviour
    {
        private static readonly string[] titles = { "Kategoriefilter", "Category Filter" };

        public FilterChangedEvent filterChangedEvent;

        // UI
        public Canvas canvas;

        public Text titleText;

        public RectTransform content;

        private int language;

        public GameObject filterItemTemplate;

        private List<FilterItem> filterItems = new List<FilterItem>();

        void Update()
        {
            // Make sure user is on Android platform
            if (Application.platform == RuntimePlatform.Android)
            { 
                // Check if Back was pressed this frame
                if (Input.GetKeyDown(KeyCode.Escape))
                    Hide();
            }
        }

        public void Show(int language, Category[] categoryCollection, HashSet<int> unselectedCategories)
        {
            Debug.Log("CategoryFilter::Show " + language);

            this.language = language;

            titleText.text = titles[language];

            canvas.enabled = true;

            SetFilters(categoryCollection, unselectedCategories);
        }

        public void Hide()
        {
            Debug.Log("CategoryFilter::Hide");

            canvas.enabled = false;
            gameObject.SetActive(false);

            foreach (FilterItem item in filterItems)
            {
                item.gameObject.SetActive(false);
            }
        }

        public void Filter()
        {
            List<int> unselectedCategories = new List<int>();

            int index = 0;

            foreach (FilterItem item in filterItems)
            {
                if (item.gameObject.activeInHierarchy && item.IsUnselected())
                {
                    unselectedCategories.Add(index);
                }

                ++index;
            }

            filterChangedEvent?.Invoke(unselectedCategories);

            Hide();
        }

        private void SetFilters(Category[] categoryCollection, HashSet<int> unselectedCategories)
        {
            int index = 0;

            foreach (Category category in categoryCollection)
            {
                SetFilter(index, language == 0 ? category.name_de : category.name_en, unselectedCategories.Contains(index));
                ++index;
            }
        }

        private void SetFilter(int index, string name, bool unselected)
        {
            FilterItem item;

            if (filterItems.Count > index)
            {
                item = filterItems[index];
            }
            else
            {
                GameObject gameObject = Instantiate(filterItemTemplate);
                gameObject.transform.SetParent(content, false);
                gameObject.transform.localScale = Vector3.one;

                item = gameObject.GetComponent<FilterItem>();

                filterItems.Add(item);
            }

            item.SetName(name);
            item.SetUnselected(unselected);

            item.gameObject.SetActive(true);
        }
    }
}
