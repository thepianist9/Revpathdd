using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HistocachingII
{
    [Serializable]
    public class POISubdocument
    {
        public float image_aspect_ratio;

        public string image_url;
        public string description_de;
        public string description_en;
        public string caption_de;
        public string caption_en;
    }

    [Serializable]
    public class POI
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

        public int image_view_distance;

        public POISubdocument[] documents;
    }

    [Serializable]
    public class POICollection
    {
        public POI[] data;
    }

    [Serializable]
    public class POIDocument
    {
        public POI data;
    }

    [Serializable]
    public class Catalog
    {
        public string name_de;
        public string name_en;

        public POI[] pois;
    }

    [Serializable]
    public class CatalogCollection
    {
        public Catalog[] data;
    }

    public static class NetworkManager
    {
        private const string baseURL = "https://hcii-api.omdat.id/v1";
        // TODO save this in config file
        private const string apiToken = "JRdKcl4Dn2xCjpykv6SLhZLDF2lki8gOMeYXEryFNzHAwX1CZpR3pSic6a7XWVdO";

        private const string histocachePath = "pois";
        private const string categoryPath = "categories";

        private static IEnumerator GetRequest(string url, Action<UnityWebRequest> callback)
        {
            using (UnityWebRequest req = UnityWebRequest.Get(url))
            {
                req.SetRequestHeader("Authorization", String.Format("Bearer {0}", apiToken));

                // Send the request and wait for a response
                yield return req.SendWebRequest();
                callback(req);
            }
        }

        // Obsolete
        public static IEnumerator GetPOICollection(Action<POI[]> callback)
        {
            return GetRequest(String.Format("{0}/{1}", baseURL, histocachePath), (UnityWebRequest req) =>
            {
                switch (req.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error: " + req.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + req.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("Received: " + req.downloadHandler.text);

                        POICollection poiCollection = JsonUtility.FromJson<POICollection>(req.downloadHandler.text);

                        callback(poiCollection?.data);
                        break;
                }
            });
        }

        public static IEnumerator GetPOIDocument(Action<POI> callback, string poiId)
        {
            return GetRequest(String.Format("{0}/{1}/{2}", baseURL, histocachePath, poiId), (UnityWebRequest req) =>
            {
                switch (req.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error: " + req.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + req.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("Received: " + req.downloadHandler.text);

                        POIDocument poiDocument = JsonUtility.FromJson<POIDocument>(req.downloadHandler.text);

                        callback(poiDocument?.data);
                        break;
                }
            });
        }

        public static IEnumerator GetCatalogCollection(Action<Catalog[]> callback)
        {
            return GetRequest(String.Format("{0}/{1}", baseURL, categoryPath), (UnityWebRequest req) =>
            {
                switch (req.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error: " + req.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + req.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("Received: " + req.downloadHandler.text);

                        CatalogCollection catalogCollection = JsonUtility.FromJson<CatalogCollection>(req.downloadHandler.text);

                        callback(catalogCollection?.data);
                        break;
                }
            });
        }

        //

        public static IEnumerator GetHistocache(string id, Action<string> callback)
        {
            return GetRequest(String.Format("{0}/{1}/{2}", baseURL, histocachePath, id), (UnityWebRequest req) =>
            {
                switch (req.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error: " + req.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + req.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("Received: " + req.downloadHandler.text);

                        callback(req.downloadHandler.text);
                        break;
                }
            });
        }

        public static IEnumerator GetHistocacheCollection(Action<string> callback)
        {
            return GetRequest(String.Format("{0}/{1}", baseURL, histocachePath), (UnityWebRequest req) =>
            {
                switch (req.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error: " + req.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + req.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("Received: " + req.downloadHandler.text);

                        callback(req.downloadHandler.text);
                        break;
                }
            });
        }

        public static IEnumerator GetCategoryCollection(Action<string> callback)
        {
            return GetRequest(String.Format("{0}/{1}", baseURL, categoryPath), (UnityWebRequest req) =>
            {
                switch (req.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error: " + req.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + req.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("Received: " + req.downloadHandler.text);

                        callback(req.downloadHandler.text);
                        break;
                }
            });
        }
    }
}
