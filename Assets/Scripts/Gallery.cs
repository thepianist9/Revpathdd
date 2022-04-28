using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class Gallery : MonoBehaviour
    {
        private static readonly string[] titles = { "Galerie", "Gallery" };
        private static Gallery _Instance;
        public static Gallery Instance { get { return _Instance; } }
    
        // UI
        public Canvas canvas;

        public Documents documents;
        
        public Text titleText;

        public RectTransform content;

        public RecyclingListView listView;

        // Language
        private int language;
        private string t;

        // Game Object
        public GameObject categoryItemTemplate;
        public GameObject histocacheItemTemplate;
        

        private Category[] categoryCollection;
        private Tag[] tags;
        public List<Tag> selectedTags;
        public HashSet<string> filteredPois = new HashSet<string>();
        List<String> tagPois = new List<string>();

        public List<Histocache> histocachesData;
        public Dictionary<string, List<string>> linkedTags = new Dictionary<string, List<string>>();
        private HashSet<int> unselectedCategories = new HashSet<int>();

        // Filter
        public CategoryFilter filter;

       
    

        // Start is called before the first frame update
        void Start()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }
            

            listView.IsHeaderCallback = IsHeader;

            listView.HeaderItemCallback = GetHeader;
            listView.ItemCallback = GetItem;
            
        }

#if UNITY_ANDROID
        void Update()
        {
            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
                Hide();
        }
#endif

        public void Show(int language)
        {
            Debug.Log("Gallery::Show " + language);

            this.language = language;

            titleText.text = titles[language];

            canvas.enabled = true;

            SetGallery();
            SetTags();

        }

        public void Hide()
        {
            Debug.Log("Gallery::Hide");
            // FullScreen.Instance.FSCR();

            canvas.enabled = false;
            gameObject.SetActive(false);
        }

        public void OnFilter()
        {
            if (categoryCollection == null)
                return;

            filter.gameObject.SetActive(true);
            filter.Show(language, categoryCollection, unselectedCategories);
        }

        public void OnFilterChanged(List<int> unselectedCategories)
        {
            if (this.unselectedCategories.SetEquals(unselectedCategories))
                return;

            this.unselectedCategories = new HashSet<int>(unselectedCategories);

            SetGallery();
        }
        private void SetList()
        {
            listView.DisableAllChildren();
            filteredPois.Clear();

            int count = 0;

            if (selectedTags.Count!=0) 
            {
                foreach (Tag tag in selectedTags)
                {
                    foreach (Histocache poi in tag.pois)
                    {
                        tagPois.Add(poi._id);
                    }
                }
            }
            
            else 
                tagPois.Clear();
        

            for (int i = 0; i < categoryCollection.Length; ++i)
            {
                Category category = categoryCollection[i];

                if (unselectedCategories.Contains(i))
                    continue;
                foreach (Histocache poi in category.pois)
                {
                    filteredPois.Add(poi._id);
                    if (tagPois.Count != 0)
                    {
                        if (!tagPois.Contains(poi._id))
                            filteredPois.Remove(poi._id);
                    }
                    
                }
            }
            Debug.Log(filteredPois.Count  );
            listView.SetCount(this.categoryCollection.Length - unselectedCategories.Count, filteredPois.Count);
        }  
        
        private void SetGallery()
        {
            if (categoryCollection != null)
            {
                SetList();
            }
            else
            {
                DataManager.Instance.GetCategoryCollection((bool success, Category[] categoryCollection) =>
                {
                    if (success)
                    {
                        this.categoryCollection = categoryCollection;
        
                        SetList();
                    }
                });
            }
        }
        private void SetTags()
        {
            TagScrollViewController instance = TagScrollViewController.Instance;
            if (tags != null)
            {
                if (instance.language != language)
                {
                    instance.DestroyTags();
                    instance.LoadTagButtons(tags.ToList(), language);
                }
                
            }
            else
            {
                DataManager.Instance.GetTags((bool success, Tag[] t) =>
                {
                    if (success)
                    {
                        tags = t;
                        TagScrollViewController.Instance.LoadTagButtons(tags.ToList(), language);
                    }
                });
            }
        }

        public void ClearTags()
        {
            TagScrollViewController.Instance.ClearTags();
            selectedTags.Clear();
            SetGallery();
        }

        // private void SetList()
        // {
        //     listView.DisableAllChildren();
        //
        //     int count = 0;
        //
        //     for (int i = 0; i < categoryCollection.Length; ++i)
        //     {
        //         Category category = categoryCollection[i];
        //
        //         if (unselectedCategories.Contains(i))
        //             continue;
        //             
        //         count += category.pois.Length;
        //     }
        //
        //     listView.SetCount(this.categoryCollection.Length - unselectedCategories.Count, count);
        // }  


        public void SetTagsList(Tag tag, string upd)
        {
            if (upd == "add")
            {
                selectedTags.Add(tag);
                SetGallery();
            }
            else if(upd == "remove")
            {
                selectedTags.Remove(tag);
                SetGallery();
            }
            else
            {
                selectedTags.Clear();
                SetGallery();
            }
            
        }

        private void GetHistocache(string id, Action<Histocache> callback)
        {
            DataManager.Instance.GetHistocache(id, (bool success, Histocache histocache) =>
            {
                callback(histocache);
            });
        }

        private void OnHistocache(int categoryIndex, int histocacheIndex)
        {
			documents.gameObject.SetActive(true);
            documents.Show(language, categoryCollection[categoryIndex].pois[histocacheIndex]);
        }

        private bool IsHeader(int index)
        {
            int headerIndex = 0;

            for (int i = 0; i < categoryCollection?.Length; ++i)
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

        private void  GetItem(RecyclingListViewItem item, int rowIndex)
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

            if (selectedTags.Count > 0)
            {
                if (!tagPois.Contains(histocache._id))
                {
                    return;
                }
            }

            histocacheItem.SetTitle(language == 0 ? histocache.title_de : histocache.title_en);

            if (string.IsNullOrWhiteSpace(histocache.file_url))
            {
                GetHistocache(histocache._id, (Histocache h) =>
                {
                    if (h != null )
                    {
                        histocache.file_url = h.file_url;
                        histocache.image_aspect_ratio = h.image_aspect_ratio;
                        histocache.description_de = h.description_de;
                        histocache.description_en = h.description_en;
                        histocache.caption_de = h.caption_de;
                        histocache.caption_en = h.caption_en;
                        histocache.tags = h.tags;

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

                        histocacheItem.SetPhotoURL(h.file_url, h.image_aspect_ratio);

                        // histocacheItem.SetAR(!histocache.has_histocache_location);
                    }
                });
            }
            else
            {
                histocacheItem.SetPhotoURL(histocache.file_url, histocache.image_aspect_ratio);

                // histocacheItem.SetAR(!histocache.has_histocache_location);
            }
        }
    }
}
