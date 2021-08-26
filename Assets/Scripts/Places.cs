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
        public Text titleText;

        public RectTransform content;

        public RecyclingListView listView;

        // Language
        private int language;

        // Game Object
        public GameObject categoryItemTemplate;
        public GameObject histocacheItemTemplate;

        private Category[] categoryCollection;

        // Start is called before the first frame update
        void Start()
        {
            backButton.onClick.AddListener(Hide);

            listView.IsHeaderCallback = IsHeader;

            listView.HeaderItemCallback = GetHeader;
            listView.ItemCallback = GetItem;
        }

        void Destroy()
        {
            backButton.onClick.RemoveListener(Hide);
        }

        public void Show(int language)
        {
            Debug.Log("Places::Show " + language);

            this.language = language;

            titleText.text = titles[language];

            canvas.enabled = true;

            content.gameObject.SetActive(true);

            GetCategoryCollection();
        }

        public void Hide()
        {
            Debug.Log("Places::Hide");

            canvas.enabled = false;

            content.gameObject.SetActive(false);
        }

        private void GetCategoryCollection()
        {
            if (categoryCollection != null)
                return;

            DataManager.Instance.GetCategoryCollection((Category[] categoryCollection) =>
            {
                this.categoryCollection = categoryCollection;

                // m_IsLoadingPOI = false;

                int index = 0;

                foreach (Category category in categoryCollection)
                {
                    // SetCategory(i);

                    // for (int j = 0; j < catalog.pois.Length; ++j)
                    // {
                    //     SetPOI(i, j, index++);
                    // }

                    index += category.pois.Length;
                }

                listView.SetCount(this.categoryCollection.Length, index);
            });
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

        void OnHistocache(int categoryIndex, int histocacheIndex)
        {
            documents.Show(language, categoryCollection[categoryIndex].pois[histocacheIndex]);
        }

        private bool IsHeader(int index)
        {
            int headerIndex = 0;

            for (int i = 0; i < categoryCollection.Length; ++i)
            {
                if (index == headerIndex) return true;
                if (index < headerIndex) return false;

                headerIndex += 1 + categoryCollection[i].pois.Length;
            }

            return false;
        }

        private void GetHeader(RecyclingListViewItem item, int rowIndex)
        {
            if (item.CurrentRow == rowIndex)
                return;

            int categoryIndex = 0;

            for (int i = 0; i < categoryCollection.Length; ++i)
            {
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
            if (item.CurrentRow == rowIndex)
                return;

            int categoryIndex = 0;
            int histocacheIndex = rowIndex;
            
            for (int i = 0; i < categoryCollection.Length; ++i)
            {
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

                        histocache.documents = h.documents;

                        categoryCollection[categoryIndex].pois[histocacheIndex] = histocache;

                        if (item.CurrentRow != rowIndex)
                            return;

                        histocacheItem.SetPhotoURL(histocache.image_url, histocache.image_aspect_ratio);
                    }
                });
            }
            else
            {
                histocacheItem.SetPhotoURL(histocache.image_url, histocache.image_aspect_ratio);
            }
        }
    }
}
