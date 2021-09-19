using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HistocachingII
{
    [Serializable]
    public class Meta
    {
        public string updated_at;
    }

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
        public string _id;

        public bool is_displayed_on_table;

        public bool has_histocache_location;

        public float lat;
        public float @long;

        public bool has_viewpoint_location;

        public float viewpoint_lat;
        public float viewpoint_long;

        public string viewpoint_image_url;
        public float viewpoint_image_aspect_ratio;
        public float viewpoint_image_height;
        public float viewpoint_image_offset;
        public float viewpoint_image_vertical_offset;

        public string title_de;
        public string title_en;

        public float image_aspect_ratio;

        public string image_url;
        public string description_de;
        public string description_en;
        public string caption_de;
        public string caption_en;

        public string add_info_url;

        public Document[] documents;

        public string updated_at;

        public bool updatedAtChecked;
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

        public Meta meta;
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
        public Meta meta;
    }

    [Serializable]
    public class JsonHistocacheDictionary : ISerializationCallbackReceiver
    {
        public List<string> _keys = new List<string>();
        public List<Histocache> _values = new List<Histocache>();
        
        // Unity does not know how to serialize a Dictionary
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

        // Data from GetHistocacheCollection
        private Histocache[] histocacheCollection;
        private Meta histocacheCollectionMeta;
        private bool histocacheCollectionMetaChecked;

        // Data from GetCategoryCollection
        private Category[] categoryCollection;
        private Meta categoryCollectionMeta;
        private bool categoryCollectionMetaChecked;

        // Data from GetHistocache
        public Dictionary<string, Histocache> histocacheDictionary = new Dictionary<string, Histocache>();

        // GetHistocache processes and their callbacks
        private Dictionary<string, List<Action<Histocache>>> histocacheGetProcess = new Dictionary<string, List<Action<Histocache>>>();

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
                    string text = File.ReadAllText(histocacheCollectionPath);

                    JsonHistocacheCollection json = JsonUtility.FromJson<JsonHistocacheCollection>(text);

                    histocacheCollection = json?.data;
                    histocacheCollectionMeta = json?.meta;
                }

                if (File.Exists(categoryCollectionPath))
                {
                    string text = File.ReadAllText(categoryCollectionPath);

                    JsonCategoryCollection json = JsonUtility.FromJson<JsonCategoryCollection>(text);

                    categoryCollection = json?.data;
                    categoryCollectionMeta = json?.meta;
                }

                if (File.Exists(histocachePath))
                {
                    string text = File.ReadAllText(histocachePath);

                    histocacheDictionary = JsonUtility.FromJson<JsonHistocacheDictionary>(text)?.dictionary;

                    foreach (Histocache histocache in histocacheDictionary.Values)
                    {
                        histocache.updatedAtChecked = false;
                    }
                }

                DontDestroyOnLoad(gameObject);
            }
        }

        public void ClearCache()
        {
            histocacheCollection = null;
            histocacheCollectionMeta = null;
            histocacheCollectionMetaChecked = false;

            categoryCollection = null;
            categoryCollectionMeta = null;
            categoryCollectionMetaChecked = false;

            histocacheDictionary.Clear();

            if (File.Exists(histocacheCollectionPath))
                File.Delete(histocacheCollectionPath);

            if (File.Exists(categoryCollectionPath))
                File.Delete(categoryCollectionPath);

            if (File.Exists(histocachePath))
                File.Delete(histocachePath);
        }

        void OnApplicationPause()
        {
            if (histocacheCollection?.Length > 0)
            {
                string text = JsonUtility.ToJson(new JsonHistocacheCollection { data = histocacheCollection, meta = histocacheCollectionMeta });

                File.WriteAllText(histocacheCollectionPath, text);
            }

            if (categoryCollection?.Length > 0)
            {
                string text = JsonUtility.ToJson(new JsonCategoryCollection { data = categoryCollection, meta = categoryCollectionMeta });

                File.WriteAllText(categoryCollectionPath, text);
            }

            if (histocacheDictionary.Count > 0)
            {
                string text = JsonUtility.ToJson(new JsonHistocacheDictionary { dictionary = histocacheDictionary});

                File.WriteAllText(histocachePath, text);
            }
        }

        void OnApplicationQuit()
        {
            if (histocacheCollection?.Length > 0)
            {
                string text = JsonUtility.ToJson(new JsonHistocacheCollection { data = histocacheCollection, meta = histocacheCollectionMeta });

                File.WriteAllText(histocacheCollectionPath, text);
            }

            if (categoryCollection?.Length > 0)
            {
                string text = JsonUtility.ToJson(new JsonCategoryCollection { data = categoryCollection, meta = categoryCollectionMeta });

                File.WriteAllText(categoryCollectionPath, text);
            }

            if (histocacheDictionary.Count > 0)
            {
                string text = JsonUtility.ToJson(new JsonHistocacheDictionary { dictionary = histocacheDictionary});

                File.WriteAllText(histocachePath, text);
            }
        }

        public void GetHistocache(string id, Action<Histocache> callback)
        {
            if (histocacheDictionary.TryGetValue(id, out Histocache histocache))
            {
                if (histocache.updatedAtChecked)
                {
                    callback(histocache);
                }
                else
                {
                    Debug.Log("DataManager::GetHistocache update " + id);

                    if (histocacheGetProcess.TryGetValue(id, out List<Action<Histocache>> callbacks))
                    {
                        callbacks.Add(callback);
                    }
                    else
                    {
                        List<Action<Histocache>> c = new List<Action<Histocache>>();
                        c.Add(callback);

                        histocacheGetProcess[id] = c;

                        StartCoroutine(NetworkManager.GetHistocache(id, (string data) =>
                        {
                            if (!string.IsNullOrWhiteSpace(data))
                            {
                                histocache = JsonUtility.FromJson<JsonHistocache>(data)?.data;
                                histocache._id = id;
                            }

                            histocache.updatedAtChecked = true;

                            histocacheDictionary[id] = histocache;

                            List<Action<Histocache>> c = histocacheGetProcess[id];
                            histocacheGetProcess.Remove(id);

                            foreach (Action<Histocache> callback in c)
                            {
                                callback(histocache);
                            }

                        }, histocache?.updated_at));
                    }
                }
            }
            else
            {
                Debug.Log("DataManager::GetHistocache create" + id);

                if (histocacheGetProcess.TryGetValue(id, out List<Action<Histocache>> callbacks))
                {
                    callbacks.Add(callback);
                }
                else
                {
                    List<Action<Histocache>> c = new List<Action<Histocache>>();
                    c.Add(callback);

                    histocacheGetProcess[id] = c;

                    StartCoroutine(NetworkManager.GetHistocache(id, (string data) =>
                    {
                        // TODO
                        Histocache histocache = JsonUtility.FromJson<JsonHistocache>(data)?.data;
                        histocache._id = id;
                        histocache.updatedAtChecked = true;

                        histocacheDictionary[id] = histocache;

                        List<Action<Histocache>> c = histocacheGetProcess[id];
                        histocacheGetProcess.Remove(id);

			            foreach (Action<Histocache> callback in c)
                        {
                            callback(histocache);
                        }
                    }));
                }
            }
        }

        public void GetHistocacheCollection(Action<Histocache[]> callback)
        {
            if (histocacheCollectionMetaChecked)
            {
                callback(histocacheCollection);
            }
            else
            {
                Debug.Log("DataManager::GetHistocacheCollection");

                StartCoroutine(NetworkManager.GetHistocacheCollection((bool success, string data) =>
                {
                    if (success)
                    {
                        if (!string.IsNullOrWhiteSpace(data))
                        {
                            JsonHistocacheCollection json = JsonUtility.FromJson<JsonHistocacheCollection>(data);

                            histocacheCollection = json?.data;
                            histocacheCollectionMeta = json?.meta;
                        }

                        histocacheCollectionMetaChecked = true;

                        callback(histocacheCollection);
                    }
                    else
                    {
                        callback(null);
                    }

                }, histocacheCollectionMeta?.updated_at));
            }
        }

        public void GetCategoryCollection(Action<Category[]> callback)
        {
            if (categoryCollectionMetaChecked)
            {
                callback(categoryCollection);
            }
            else
            {
                Debug.Log("DataManager::GetCategoryCollection");

                StartCoroutine(NetworkManager.GetCategoryCollection((string data) =>
                {
                    if (!string.IsNullOrWhiteSpace(data))
                    {
                        JsonCategoryCollection json = JsonUtility.FromJson<JsonCategoryCollection>(data);

                        categoryCollection = json?.data;
                        categoryCollectionMeta = json?.meta;
                    }

                    categoryCollectionMetaChecked = true;

                    callback(categoryCollection);

                }, categoryCollectionMeta?.updated_at));
            }
        }
    }
}
