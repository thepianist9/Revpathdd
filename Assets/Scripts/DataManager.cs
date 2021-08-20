using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HistocachingII
{
    public class DataManager : MonoBehaviour
    {
        private static DataManager _Instance;
        public static DataManager Instance { get { return _Instance; } }

        public bool ready = false;

        private List<POI> histocacheCollection = new List<POI>();

        // static string filePath = Application.persistentDataPath + "/" + 
        //         "data" + "/";
    
        void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
                
                string filePath = Application.persistentDataPath + "/" + "data" + "/";

                if (File.Exists(filePath))
                {
                    string data = File.ReadAllText(filePath);
                    ParseData(data);
                }
                else
                {
                    GetData();
                }

                DontDestroyOnLoad(gameObject);
            }
        }

        private void ParseData(string data)
        {
        }

        private void GetData()
        {
            StartCoroutine(NetworkManager.GetPOICollection((POI[] histocacheCollection) =>
            {
                if (histocacheCollection != null)
                {
                    this.histocacheCollection = new List<POI>(histocacheCollection);

                    ready = true;

                    // File.WriteAllText(filePath);
                }
            }));
        }

        public ref readonly List<POI> GetHistocacheCollection()
        {
            return ref histocacheCollection;
        }
    }
}
