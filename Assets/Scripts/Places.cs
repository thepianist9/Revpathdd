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

        public Documents poiDetail;
        
        public Button backButton;
        public Text titleText;

        public RectTransform content;

        // Language
        private int language;

        // Game Object
        public GameObject categoryItemTemplate;
        public GameObject poiItemTemplate;

        private List<GameObject> categoryItems = new List<GameObject>();
        private List<GameObject> poiItems = new List<GameObject>();

        private NetworkManager networkManager = new NetworkManager();

        private List<Catalog> catalogCollection = new List<Catalog>();

        // Start is called before the first frame update
        void Start()
        {
            backButton.onClick.AddListener(Hide);
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

            GetCatalogCollection();

            canvas.enabled = true;
        }

        public void Hide()
        {
            Debug.Log("Places::Hide");

            canvas.enabled = false;
        }

        void GetCatalogCollection()
        {
            // if (m_IsLoadingPOI)
                // return;

            // m_IsLoadingPOI = true;

            this.catalogCollection.Clear();

            StartCoroutine(networkManager.GetCatalogCollection((Catalog[] catalogCollection) =>
            {
                // m_IsLoadingPOI = false;

                for (int i = 0; i < catalogCollection?.Length; ++i)
                {
                    Catalog catalog = catalogCollection[i];

                    this.catalogCollection.Add(catalog);
                }

                int index = 0;

                for (int i = 0; i < this.catalogCollection.Count; ++i)
                {
                    Catalog catalog = this.catalogCollection[i];

                    SetCategory(i);

                    for (int j = 0; j < catalog.pois.Length; ++j)
                    {
                        SetPOI(i, j, index++);
                    }
                }
            }));
        }

        void GetPOIDocument(Action<POI> callback, string poiId)
        {
            // if (m_IsLoadingPOIDocument)
                // return;

            // m_IsLoadingPOIDocument = true;

            StartCoroutine(networkManager.GetPOIDocument((POI poi) =>
            {
                // m_IsLoadingPOIDocument = false;

                callback(poi);

            }, poiId));
        }

        void SetCategory(int categoryIndex)
        {
            GameObject gameObject;

            if (categoryItems.Count > categoryIndex)
            {
                gameObject = categoryItems[categoryIndex];
            }
            else
            {
                gameObject = Instantiate(categoryItemTemplate);
                gameObject.transform.SetParent(content, false);
                gameObject.transform.localScale = new Vector3(1, 1, 1);

                categoryItems.Add(gameObject);
            }

            gameObject.SetActive(true);

            Catalog catalog = catalogCollection[categoryIndex];

            string title = "\n" + (language == 0 ? catalog.name_de : catalog.name_en);

            CategoryItem categoryItem = gameObject.GetComponent<CategoryItem>();
            categoryItem.SetTitle(title);
        }

        void SetPOI(int categoryIndex, int poiIndex, int index)
        {
            GameObject gameObject;

            if (poiItems.Count > index)
            {
                gameObject = poiItems[index];
            }
            else
            {
                gameObject = Instantiate(poiItemTemplate);
                gameObject.transform.SetParent(content, false);
                gameObject.transform.localScale = new Vector3(1, 1, 1);

                poiItems.Add(gameObject);
            }

            gameObject.SetActive(true);

            Button button = gameObject.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnPOIItem(categoryIndex, poiIndex));

            POI poi = catalogCollection[categoryIndex].pois[poiIndex];

            POIItem poiItem = gameObject.GetComponent<POIItem>();
            poiItem.SetTitle(language == 0 ? poi.title_de : poi.title_en);

            GetPOIDocument((POI p) =>
            {    
                if (p != null)
                {
                    poi.image_url = p.image_url;
                    poi.image_height = p.image_height;
                    poi.description_de = p.description_de;
                    poi.description_en = p.description_en;
                    poi.caption_de = p.caption_de;
                    poi.caption_en = p.caption_en;

                    poi.documents = p.documents;

                    catalogCollection[categoryIndex].pois[poiIndex] = poi;

                    poiItem.SetPhotoURL(poi.image_url, poi.image_aspect_ratio);
                }

            }, poi.id);
        }

        void OnPOIItem(int categoryIndex, int poiIndex)
        {
            poiDetail.Show(language, catalogCollection[categoryIndex].pois[poiIndex]);
        }
    }
}