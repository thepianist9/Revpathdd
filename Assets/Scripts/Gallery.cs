using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class Gallery : MonoBehaviour
    {
        private static readonly string[] titles = { "Galerie", "Gallery" };
    
        // UI
        public Canvas canvas;

        public Documents documents;
        
        public Text titleText;

        public RectTransform content;

        public RecyclingListView listView;

        // Language
        private int language;

        // Game Object
        public GameObject categoryItemTemplate;
        public GameObject histocacheItemTemplate;

        private Category[] categoryCollection;

        private HashSet<int> unselectedCategories = new HashSet<int>();

        // Filter
        public CategoryFilter filter;

        // Start is called before the first frame update
        void Start()
        {
            listView.IsHeaderCallback = IsHeader;

            listView.HeaderItemCallback = GetHeader;
            listView.ItemCallback = GetItem;
        }

        public void Show(int language)
        {
            Debug.Log("Gallery::Show " + language);

            this.language = language;

            titleText.text = titles[language];

            canvas.enabled = true;

            content.gameObject.SetActive(true);

            SetGallery();
        }

        public void Hide()
        {
            Debug.Log("Gallery::Hide");

            canvas.enabled = false;

            content.gameObject.SetActive(false);
        }

        public void OnFilter()
        {
            if (categoryCollection == null)
                return;

            filter.Show(language, categoryCollection, unselectedCategories);
        }

        public void OnFilterChanged(List<int> unselectedCategories)
        {
            if (this.unselectedCategories.SetEquals(unselectedCategories))
                return;

            this.unselectedCategories = new HashSet<int>(unselectedCategories);

            SetGallery();
        }

        private void SetGallery()
        {
            if (categoryCollection != null)
            {
                SetList();
            }
            else
            {
                DataManager.Instance.GetCategoryCollection((Category[] categoryCollection) =>
                {
                    this.categoryCollection = categoryCollection;

                    SetList();
                });
            }
        }

        private void SetList()
        {
            listView.DisableAllChildren();

            int count = 0;

            for (int i = 0; i < categoryCollection.Length; ++i)
            {
                Category category = categoryCollection[i];

                if (unselectedCategories.Contains(i))
                    continue;
                    
                count += category.pois.Length;
            }

            listView.SetCount(this.categoryCollection.Length - unselectedCategories.Count, count);
        }

        private void GetHistocache(string id, Action<Histocache> callback)
        {
            DataManager.Instance.GetHistocache(id, (Histocache histocache) =>
            {
                callback(histocache);
            });
        }

        private void OnHistocache(int categoryIndex, int histocacheIndex)
        {
            documents.Show(language, categoryCollection[categoryIndex].pois[histocacheIndex]);
        }

        private bool IsHeader(int index)
        {
            int headerIndex = 0;

            for (int i = 0; i < categoryCollection.Length; ++i)
            {
                if (unselectedCategories.Contains(i))
                    continue;

                if (index == headerIndex) return true;
                if (index < headerIndex) return false;

                headerIndex += 1 + categoryCollection[i].pois.Length;
            }

            return false;
        }

        private void GetHeader(RecyclingListViewItem item, int rowIndex)
        {
            // if (item.CurrentRow == rowIndex)
            //     return;

            int categoryIndex = 0;

            for (int i = 0; i < categoryCollection.Length; ++i)
            {
                if (unselectedCategories.Contains(i))
                    continue;

                if (rowIndex == 0)
                {
                    categoryIndex = i;
                    break;
                }

                rowIndex -= 1; // header
                rowIndex -= categoryCollection[i].pois.Length; // items
            }

            Category category = categoryCollection[categoryIndex];

            string title = "\n" + (language == 0 ? category.name_de : category.name_en);

            var categoryItem = item as CategoryItem;
            categoryItem.SetTitle(title);
        }

        private void GetItem(RecyclingListViewItem item, int rowIndex)
        {
            // if (item.CurrentRow == rowIndex)
            //     return;

            int categoryIndex = 0;
            int histocacheIndex = rowIndex;
            
            for (int i = 0; i < categoryCollection.Length; ++i)
            {
                if (unselectedCategories.Contains(i))
                    continue;
                
                histocacheIndex -= 1; // header
                
                if (histocacheIndex < categoryCollection[i].pois.Length)
                {      
                    categoryIndex = i;
                    break;
                }

                histocacheIndex -= categoryCollection[i].pois.Length; // items
            }

            var button = item.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnHistocache(categoryIndex, histocacheIndex));
            
            var histocacheItem = item as HistocacheItem;

            Histocache histocache = categoryCollection[categoryIndex].pois[histocacheIndex];

            histocacheItem.SetTitle(language == 0 ? histocache.title_de : histocache.title_en);

            if (string.IsNullOrWhiteSpace(histocache.image_url))
            {
                GetHistocache(histocache._id, (Histocache h) =>
                {
                    if (h != null)
                    {
                        histocache.image_url = h.image_url;
                        histocache.image_aspect_ratio = h.image_aspect_ratio;
                        histocache.description_de = h.description_de;
                        histocache.description_en = h.description_en;
                        histocache.caption_de = h.caption_de;
                        histocache.caption_en = h.caption_en;

                        histocache.viewpoint_image_url = h.viewpoint_image_url;
                        histocache.viewpoint_image_aspect_ratio = h.viewpoint_image_aspect_ratio;
                        histocache.viewpoint_image_height = h.viewpoint_image_height;
                        histocache.viewpoint_image_offset = h.viewpoint_image_offset;
                        histocache.viewpoint_image_vertical_offset = h.viewpoint_image_vertical_offset;

                        histocache.is_displayed_on_table = h.is_displayed_on_table;
                        histocache.has_histocache_location = h.has_histocache_location;
                        histocache.has_viewpoint_location = h.has_viewpoint_location;

                        histocache.add_info_url = h.add_info_url;

                        histocache.documents = h.documents;

                        categoryCollection[categoryIndex].pois[histocacheIndex] = histocache;

                        if (item.CurrentRow != rowIndex)
                            return;

                        histocacheItem.SetPhotoURL(h.image_url, h.image_aspect_ratio);

                        // histocacheItem.SetAR(!histocache.has_histocache_location);
                    }
                });
            }
            else
            {
                histocacheItem.SetPhotoURL(histocache.image_url, histocache.image_aspect_ratio);

                // histocacheItem.SetAR(!histocache.has_histocache_location);
            }
        }
    }
}
