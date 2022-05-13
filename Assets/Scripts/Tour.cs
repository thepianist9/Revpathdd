using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class TourPOI
    {
        public string _id;
        public string title_en;
        public string title_de;
        public float lat;
        public float @long;

        public TourPOI(string _id, string title_en, string title_de, float lat, float @long)
        {
            this._id = _id;
            this.title_en = title_en;
            this.title_de = title_de;
            this.lat = lat;
            this.@long = @long;
        }
    }
    public class Tour : MonoBehaviour
    {
        private static readonly string[] titles = { "Tour", "Tour" };
    
        // UI
        public Canvas canvas;
        [SerializeField] private Canvas TourCanvas;

        public Documents documents;
        
        public Text titleText;

        public RecyclingListView listView;
        private  float maxApproachingSqrDistance = 225f;
       

        [SerializeField] private Button stopNavigationButton;
        
        
        [SerializeField] private GameObject panel;
        [SerializeField] private Button dirPanelButton;

        // Language
        private int language;
        


       
        public void Show(int language)
        {
            Debug.Log("Gallery::Show " + language);

            this.language = language;

            titleText.text = titles[language];

            canvas.enabled = true;
            

        }

        // Filter
        public CategoryFilter filter;

   

        // Start is called before the first frame update
        void Start()
        {
            

        }

        public void Hide()
        {
            Debug.Log("Gallery::Hide");

            canvas.enabled = false;
        }




        public void StartTourNavigation()
        {
            List<TourPOI> tourCollection = new List<TourPOI>();
            
            tourCollection.Add(new TourPOI("60b9ee6c2e4fc867707516a2",
                "Andreas Schubert Building (built for nuclear physics)",
                "Andreas-Schubert-Bau (errichtet für die Kernphysik)", (float) 51.02963099289087, (float)13.740433955294584));
        
            tourCollection.Add(new TourPOI("60b9ee6c2e4fc867707516a2",
                "Andreas Schubert Building (built for nuclear physics)",
                "Andreas-Schubert-Bau (errichtet für die Kernphysik)", (float) 51.02963099289087, (float)13.740433955294584));
            tourCollection.Add(new TourPOI("61a2b4d7df62c529701a33cc",
                "Mierdel Building (microelectronics - technical centre)",
                "Mierdelbau (Mikroelektronik - Technikum)", (float) 51.0505432339807, (float)13.737297867447133));
            tourCollection.Add(new TourPOI("61a77a425760797212ca7175",
                "Lenne Crossover",
                "Lenne Platz", (float) 51.038404637511555, (float) 13.746704586992315));
            tourCollection.Add(new TourPOI("61f313de6ea1897db94d87e8",
                "TU Dresden",
                "TÜ Dresden", (float) 51.04895308130884, (float) 13.729741246461863));
            
            DirectionsFactory.Instance.TourHandler(tourCollection);
            TourCanvas.gameObject.SetActive(true);
            Hide();
            dirPanelButton.onClick.AddListener(DisplayDirPanel);
            stopNavigationButton.onClick.AddListener(StopNavigation);
            
        }
        private void DisplayDirPanel()
        {
            //TODO: fix active state
            panel.gameObject.SetActive(!panel.gameObject.activeSelf);

        }

        private void StopNavigation()
        {
            canvas.gameObject.SetActive(false); 
            DirectionsFactory.Instance.DestroyDirections();
        }

    
    }
}
