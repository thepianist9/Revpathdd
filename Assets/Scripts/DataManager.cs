using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HistocachingII
{
    [Serializable]
    public class Histocache
    {
        public string id;

        public float lat;
        public float @long;

        public string title_de;
        public string title_en;

        public float image_aspect_ratio;

        public string image_url;
        public string description_de;
        public string description_en;
        public string caption_de;
        public string caption_en;

        public int image_height;

        // public POISubdocument[] documents;
    }

    [Serializable]
    public class HistocacheCollection
    {
        public Histocache[] data;
    }

    public class DataManager : MonoBehaviour
    {
        private static DataManager _Instance;
        public static DataManager Instance { get { return _Instance; } }

        public bool ready = false;

        private Histocache[] histocacheCollection;

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

                    histocacheCollection = ParseData(data)?.data;

                    ready = true;
                }
                else
                {
                    GetData();
                }

                DontDestroyOnLoad(gameObject);
            }
        }

        private HistocacheCollection ParseData(string data)
        {
            return JsonUtility.FromJson<HistocacheCollection>(data);
        }

        private void GetData()
        {
            StartCoroutine(NetworkManager.GetHistocacheCollection((string data) =>
            {
                histocacheCollection = ParseData(data)?.data;

                ready = true;

                string filePath = Application.persistentDataPath + "/" + "data" + "/";

                File.WriteAllText(filePath, data);
            }));
        }

        public ref readonly Histocache[] GetHistocacheCollection()
        {
            return ref histocacheCollection;
        }

        public ref Histocache[] GetMutableHistocacheCollection()
        {
            return ref histocacheCollection;
        }
    }
}
