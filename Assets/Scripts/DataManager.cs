using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HistocachingII
{
    [Serializable]
    public class Document
    {
        public float image_aspect_ratio;

        public string image_url;
        public string description_de;
        public string description_en;
        public string caption_de;
        public string caption_en;
    }

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

        public Document[] documents;
    }

    [Serializable]
    public class JsonHistocache
    {
        public Histocache data;
    }

    [Serializable]
    public class JsonHistocacheCollection
    {
        public Histocache[] data;
    }

    [Serializable]
    public class Category
    {
        public string name_de;
        public string name_en;

        public Histocache[] pois;
    }

    [Serializable]
    public class JsonCategoryCollection
    {
        public Category[] data;
    }

    [Serializable]
    public class JsonHistocacheDictionary : ISerializationCallbackReceiver
    {
        public List<string> _keys = new List<string>();
        public List<Histocache> _values = new List<Histocache>();
        
        // Unity doesn't know how to serialize a Dictionary
        public Dictionary<string, Histocache> dictionary;

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (var kvp in dictionary)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            dictionary = new Dictionary<string, Histocache>();

            for (int i = 0; i != Math.Min(_keys.Count, _values.Count); ++i)
                dictionary.Add(_keys[i], _values[i]);
        } 
    }

    public class DataManager : MonoBehaviour
    {
        private static DataManager _Instance;
        public static DataManager Instance { get { return _Instance; } }

        private string histocacheCollectionPath;
        private string categoryCollectionPath;

        private string histocachePath;

        private Histocache[] histocacheCollection;

        private Category[] categoryCollection;

        public Dictionary<string, Histocache> histocacheDictionary = new Dictionary<string, Histocache>();

        // static string filePath = Application.persistentDataPath + "/" + 
        //         "data" + "/";
    
        void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;

                string directory = Application.persistentDataPath + "/" + "data" + "/";

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                histocacheCollectionPath = directory + "histocacheCollection.dat";
                categoryCollectionPath = directory + "categoryCollection.dat";
                histocachePath = directory + "histocache.dat";
                
                if (File.Exists(histocacheCollectionPath))
                {
                    // File.Delete(histocacheCollectionPath);

                    string data = File.ReadAllText(histocacheCollectionPath);

                    histocacheCollection = JsonUtility.FromJson<JsonHistocacheCollection>(data)?.data;
                }

                if (File.Exists(categoryCollectionPath))
                {
                    // File.Delete(categoryCollectionPath);

                    string data = File.ReadAllText(categoryCollectionPath);

                    categoryCollection = JsonUtility.FromJson<JsonCategoryCollection>(data)?.data;
                }

                if (File.Exists(histocachePath))
                {
                    // File.Delete(histocachePath);

                    string data = File.ReadAllText(histocachePath);

                    histocacheDictionary = JsonUtility.FromJson<JsonHistocacheDictionary>(data)?.dictionary;
                }

                DontDestroyOnLoad(gameObject);
            }
        }

        void OnApplicationPause()
        {
            if (histocacheCollection != null && histocacheCollection.Length > 0)
            {
                string data = JsonUtility.ToJson(new JsonHistocacheCollection { data = histocacheCollection });

                File.WriteAllText(histocacheCollectionPath, data);
            }

            if (categoryCollection != null && categoryCollection.Length > 0)
            {
                string data = JsonUtility.ToJson(new JsonCategoryCollection { data = categoryCollection });

                File.WriteAllText(categoryCollectionPath, data);
            }

            if (histocacheDictionary.Count > 0)
            {
                string data = JsonUtility.ToJson(new JsonHistocacheDictionary { dictionary = histocacheDictionary});

                File.WriteAllText(histocachePath, data);
            }
        }

        void OnApplicationQuit()
        {
            if (histocacheCollection != null && histocacheCollection.Length > 0)
            {
                string data = JsonUtility.ToJson(new JsonHistocacheCollection { data = histocacheCollection });

                // Debug.Log("1 data " + data);

                File.WriteAllText(histocacheCollectionPath, data);
            }

            if (categoryCollection != null && categoryCollection.Length > 0)
            {
                string data = JsonUtility.ToJson(new JsonCategoryCollection { data = categoryCollection });

                // Debug.Log("2 data " + data);

                File.WriteAllText(categoryCollectionPath, data);
            }

            if (histocacheDictionary.Count > 0)
            {
                string data = JsonUtility.ToJson(new JsonHistocacheDictionary { dictionary = histocacheDictionary});

                // Debug.Log("3 data " + data);

                File.WriteAllText(histocachePath, data);
            }
        }

        public void GetHistocache(string id, Action<Histocache> callback)
        {
            if (histocacheDictionary.ContainsKey(id))
            {
                callback(histocacheDictionary[id]);
            }
            else
            {
                Debug.Log("GetHistocache " + id);

                StartCoroutine(NetworkManager.GetHistocache(id, (string data) =>
                {
                    Histocache histocache = JsonUtility.FromJson<JsonHistocache>(data)?.data;

                    histocacheDictionary[id] = histocache;

                    callback(histocache);
                }));
            }
        }

        public void GetHistocacheCollection(Action<Histocache[]> callback)
        {
            if (histocacheCollection != null)
            {
                callback(histocacheCollection);
            }
            else
            {
                Debug.Log("GetHistocacheCollection");

                StartCoroutine(NetworkManager.GetHistocacheCollection((string data) =>
                {
                    histocacheCollection = JsonUtility.FromJson<JsonHistocacheCollection>(data)?.data;

                    callback(histocacheCollection);
                }));
            }
        }

        public void GetCategoryCollection(Action<Category[]> callback)
        {
            if (categoryCollection != null)
            {
                callback(categoryCollection);
            }
            else
            {
                Debug.Log("GetCategoryCollection");

                StartCoroutine(NetworkManager.GetCategoryCollection((string data) =>
                {
                    categoryCollection = JsonUtility.FromJson<JsonCategoryCollection>(data)?.data;

                    callback(categoryCollection);
                }));
            }
        }
    }
}
