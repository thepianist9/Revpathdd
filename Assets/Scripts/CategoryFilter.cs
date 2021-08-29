using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class CategoryFilter : MonoBehaviour
    {
        private static readonly string[] titles = { "Kategoriefilter", "Category Filter" };

        // UI
        public Canvas canvas;

        public Button backButton;

        public Text titleText;

        public RectTransform content;

        private int language;

        public GameObject filterItemTemplate;

        private List<FilterItem> filterItems = new List<FilterItem>();

        private Category[] categoryCollection;

        private List<int> selectedCategories;

        // Start is called before the first frame update
        void Start()
        {
            backButton.onClick.AddListener(Hide);
        }

        void Destroy()
        {
            backButton.onClick.RemoveListener(Hide);
        }

        public void Show(int language, List<int> selectedCategories)
        {
            Debug.Log("CategoryFilter::Show " + language);

            this.language = language;

            canvas.enabled = true;

            content.gameObject.SetActive(true);

            this.selectedCategories = selectedCategories;

            GetCategoryCollection();
        }

        public void Hide()
        {
            Debug.Log("CategoryFilter::Hide");

            canvas.enabled = false;

            content.gameObject.SetActive(false);

            foreach (FilterItem item in filterItems)
            {
                item.gameObject.SetActive(false);
            }
        }

        private void GetCategoryCollection()
        {
            if (categoryCollection != null)
            {
                SetFilters();
            }
            else
            {
                DataManager.Instance.GetCategoryCollection((Category[] categoryCollection) =>
                {
                    this.categoryCollection = categoryCollection;

                    SetFilters();
                });
            }
        }

        void SetFilters()
        {
            int index = 0;

            foreach (Category category in categoryCollection)
            {
                SetFilter(index, language == 0 ? category.name_de : category.name_en, selectedCategories.Contains(index));
                ++index;
            }
        }

        void SetFilter(int index, string name, bool selected)
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
            item.SetSelected(selected);

            item.gameObject.SetActive(true);
        }
    }
}
