using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class Places : MonoBehaviour
    {
        private static readonly string[] titles = { "Fotogallerie", "Photo Gallery" };
    
        // UI
        public Canvas canvas;

        public Documents documents;
        
        public Button backButton;
        public Button filterButton;

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
            backButton.onClick.AddListener(Hide);
            filterButton.onClick.AddListener(OnFilter);

            filter.filterChangedEvent.AddListener(OnFilterChanged);

            listView.IsHeaderCallback = IsHeader;

            listView.HeaderItemCallback = GetHeader;
            listView.ItemCallback = GetItem;
        }

        void Destroy()
        {
            backButton.onClick.RemoveListener(Hide);
            filterButton.onClick.AddListener(OnFilter);

            filter.filterChangedEvent.RemoveListener(OnFilterChanged);
        }

        public void Show(int language)
        {
            Debug.Log("Places::Show " + language);

            this.language = language;

            titleText.text = titles[language];

            canvas.enabled = true;

            content.gameObject.SetActive(true);

            SetPlaces();
        }

        private void Hide()
        {
            Debug.Log("Places::Hide");

            canvas.enabled = false;

            content.gameObject.SetActive(false);
        }

        private void OnFilter()
        {
            if (categoryCollection == null)
                return;

            filter.Show(language, categoryCollection, unselectedCategories);
        }

        private void OnFilterChanged(List<int> unselectedCategories)
        {
            if (this.unselectedCategories.SetEquals(unselectedCategories))
                return;

            this.unselectedCategories = new HashSet<int>(unselectedCategories);

            SetPlaces();
        }

        private void SetPlaces()
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

            // m_IsLoadingPOI = false;

            int count = 0;

            for (int i = 0; i < categoryCollection.Length; ++i)
            {
                Category category = categoryCollection[i];

                // SetCategory(i);

                // for (int j = 0; j < catalog.pois.Length; ++j)
                // {
                //     SetPOI(i, j, index++);
                // }

                if (unselectedCategories.Contains(i))
                    continue;
                    
                count += category.pois.Length;
            }

            listView.SetCount(this.categoryCollection.Length - unselectedCategories.Count, count);
        }

        private void GetHistocache(string id, Action<Histocache> callback)
        {
            // if (m_IsLoadingPOIDocument)
                // return;

            // m_IsLoadingPOIDocument = true;

            DataManager.Instance.GetHistocache(id, (Histocache histocache) =>
            {
                // m_IsLoadingPOIDocument = false;

                callback(histocache);
            });
        }

        // void SetCategory(int categoryIndex)
        // {
        //     GameObject gameObject;

        //     if (categoryItems.Count > categoryIndex)
        //     {
        //         gameObject = categoryItems[categoryIndex];
        //     }
        //     else
        //     {
        //         gameObject = Instantiate(categoryItemTemplate);
        //         gameObject.transform.SetParent(content, false);
        //         gameObject.transform.localScale = new Vector3(1, 1, 1);

        //         categoryItems.Add(gameObject);
        //     }

        //     gameObject.SetActive(true);

        //     Catalog catalog = catalogCollection[categoryIndex];

        //     string title = "\n" + (language == 0 ? catalog.name_de : catalog.name_en);

        //     CategoryItem categoryItem = gameObject.GetComponent<CategoryItem>();
        //     categoryItem.SetTitle(title);
        // }

        // void SetPOI(int categoryIndex, int poiIndex, int index)
        // {
        //     GameObject gameObject;

        //     if (poiItems.Count > index)
        //     {
        //         gameObject = poiItems[index];
        //     }
        //     else
        //     {
        //         gameObject = Instantiate(poiItemTemplate);
        //         gameObject.transform.SetParent(content, false);
        //         gameObject.transform.localScale = new Vector3(1, 1, 1);

        //         poiItems.Add(gameObject);
        //     }

        //     gameObject.SetActive(true);

        //     Button button = gameObject.GetComponent<Button>();
        //     button.onClick.RemoveAllListeners();
        //     button.onClick.AddListener(() => OnPOIItem(categoryIndex, poiIndex));

        //     POI poi = catalogCollection[categoryIndex].pois[poiIndex];

        //     POIItem poiItem = gameObject.GetComponent<POIItem>();
        //     poiItem.SetTitle(language == 0 ? poi.title_de : poi.title_en);

        //     GetPOIDocument((POI p) =>
        //     {    
        //         if (p != null)
        //         {
        //             poi.image_url = p.image_url;
        //             poi.image_height = p.image_height;
        //             poi.description_de = p.description_de;
        //             poi.description_en = p.description_en;
        //             poi.caption_de = p.caption_de;
        //             poi.caption_en = p.caption_en;

        //             poi.documents = p.documents;

        //             catalogCollection[categoryIndex].pois[poiIndex] = poi;

        //             poiItem.SetPhotoURL(poi.image_url, poi.image_aspect_ratio);
        //         }

        //     }, poi.id);
        // }

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
                        if (item.CurrentRow == rowIndex)
                        {
                            histocacheItem.SetPhotoURL(h.image_url, h.image_aspect_ratio);

                            // histocacheItem.SetAR(!histocache.has_histocache_location);
                        }

                        histocache.image_url = h.image_url;
                        histocache.image_aspect_ratio = h.image_aspect_ratio;
                        histocache.description_de = h.description_de;
                        histocache.description_en = h.description_en;
                        histocache.caption_de = h.caption_de;
                        histocache.caption_en = h.caption_en;

                        histocache.has_histocache_location = h.has_histocache_location;
                        histocache.has_viewpoint_location = h.has_viewpoint_location;

                        histocache.add_info_url = h.add_info_url;

                        histocache.documents = h.documents;

                        categoryCollection[categoryIndex].pois[histocacheIndex] = histocache;
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
