using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HistocachingII
{
    [Serializable]
    public class POI
    {
        public string id;
        public float lat;
        public float @long;

        public string image_url;
        public int image_height;
        public string title_de;
        public string title_en;
        public string description_de;
        public string description_en;
        public string caption_de;
        public string caption_en;
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
        public string nameDE;
        public string nameEN;
    }

    public class NetworkManager
    {
        private const string baseURL = "https://hcii-api.omdat.id/v1";
        // TODO save this in config file
        private const string apiToken = "JRdKcl4Dn2xCjpykv6SLhZLDF2lki8gOMeYXEryFNzHAwX1CZpR3pSic6a7XWVdO";

        private const string poiCollectionPath = "pois";
        private const string catalogCollectionPath = "catalog";

        private IEnumerator PostRequest(string url, Action<UnityWebRequest> callback)
        {
            using (UnityWebRequest req = UnityWebRequest.Post(url, ""))
            {
                req.SetRequestHeader("Authorization", String.Format("Bearer {0}", apiToken));

                // Send the request and wait for a response
                yield return req.SendWebRequest();
                callback(req);
            }
        }

        public IEnumerator GetPOICollection(Action<POI[]> callback)
        {
            return PostRequest(String.Format("{0}/{1}", baseURL, poiCollectionPath), (UnityWebRequest req) =>
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

        public IEnumerator GetPOIDocument(Action<POI> callback, string poiId)
        {
            return PostRequest(String.Format("{0}/{1}/{2}", baseURL, poiCollectionPath, poiId), (UnityWebRequest req) =>
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

        public IEnumerator GetCatalogList(Action<UnityWebRequest> callback)
        {
            return PostRequest(String.Format("{0}/{1}", baseURL, catalogCollectionPath), callback);
        }
    }
}
