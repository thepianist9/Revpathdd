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
        // Development
        // private const string baseURL = "https://hcii-api.omdat.id/v1";
        // private const string apiToken = "JRdKcl4Dn2xCjpykv6SLhZLDF2lki8gOMeYXEryFNzHAwX1CZpR3pSic6a7XWVdO";

        // Production
        private const string baseURL = "https://hcapi.inf.tu-dresden.de/api/v1";
        private const string apiToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJfaWQiOiI2MTIxNjhkOGE4NzY4MTJhMThiZjU2YzIiLCJuYW1lIjoiQlN0VSIsInJvbGUiOiJvd25lciIsImlhdCI6MTYzMTc0ODQ3NiwiZXhwIjoxNjM5NTI0NDc2fQ.UZLMvTbZfytQNLuyurQ6_7ysUvQrMif5etixCePixwU";

        private const string histocachePath = "pois";
        private const string categoryPath = "categories";

        private static IEnumerator GetRequest(string url, Action<UnityWebRequest> callback)
        {
            using (UnityWebRequest req = UnityWebRequest.Get(url))
            {
                // Accept all certificate (hackish handling for TU Dresden CA)
                req.certificateHandler = new TUDCertificateHandler();

                // Development
                // req.SetRequestHeader("Authorization", String.Format("Bearer {0}", apiToken));
                // Production
                req.SetRequestHeader("x-auth-token", apiToken);

                // Send the request and wait for a response
                int retry = 3;
                while (retry-- > 0)
                {
                    yield return req.SendWebRequest();
                    if (req.error == null)
                    {
                        callback(req);
                        break;
                    }
                }
            }
        }

        public static IEnumerator GetHistocache(string id, Action<string> callback, string updatedAt = null)
        {
            string url = string.IsNullOrWhiteSpace(updatedAt) ? String.Format("{0}/{1}/{2}?updated_at={3}", baseURL, histocachePath, id, updatedAt) : String.Format("{0}/{1}/{2}", baseURL, histocachePath, id);

            return GetRequest(url, (UnityWebRequest req) =>
            {
                switch (req.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        callback(null);
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("GetHistocache error: " + req.error);
                        callback(null);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("GetHistocache HTTP Error: " + req.error);
                        callback(null);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("GetHistocache received: " + req.downloadHandler.text);
                        callback(req.downloadHandler.text);
                        break;
                }
            });
        }

        public static IEnumerator GetHistocacheCollection(Action<string> callback, string updatedAt = null)
        {
            string url = string.IsNullOrWhiteSpace(updatedAt) ? String.Format("{0}/{1}", baseURL, histocachePath) : String.Format("{0}/{1}?updated_at={2}", baseURL, histocachePath, updatedAt);

            return GetRequest(url, (UnityWebRequest req) =>
            {
                switch (req.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        callback(null);
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("GetHistocacheCollection error: " + req.error);
                        callback(null);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("GetHistocacheCollection HTTP Error: " + req.error);
                        callback(null);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("GetHistocacheCollection received: " + req.downloadHandler.text);
                        callback(req.downloadHandler.text);
                        break;
                }
            });
        }

        public static IEnumerator GetCategoryCollection(Action<string> callback, string updatedAt = null)
        {
            string url = string.IsNullOrWhiteSpace(updatedAt) ? String.Format("{0}/{1}", baseURL, categoryPath) : String.Format("{0}/{1}?updated_at={2}", baseURL, categoryPath, updatedAt);

            return GetRequest(url, (UnityWebRequest req) =>
            {
                switch (req.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        callback(null);
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("GetCategoryCollection error: " + req.error);
                        callback(null);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("GetCategoryCollection HTTP Error: " + req.error);
                        callback(null);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("GetCategoryCollection received: " + req.downloadHandler.text);
                        callback(req.downloadHandler.text);
                        break;
                }
            });
        }
    }
}
