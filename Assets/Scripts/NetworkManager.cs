using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HistocachingII
{
    public static class NetworkManager
    {
        private const int maxRetry = 3;

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
                
                int retry = 0;
                while (retry++ < maxRetry)
                {
                    // Send the request and wait for a response
                    yield return req.SendWebRequest();

                    if (req.error == null)
                    {
                        callback(req);
                        yield break;
                    }
                }

                callback(req);
            }
        }

        public static IEnumerator GetHistocache(string id, Action<bool, string> callback, string updatedAt = null)
        {
            string url = string.IsNullOrWhiteSpace(updatedAt) ? String.Format("{0}/{1}/{2}", baseURL, histocachePath, id) : String.Format("{0}/{1}/{2}?updated_at={3}", baseURL, histocachePath, id, updatedAt);

            Debug.Log("NetworkManager::GetHistocache URL: " + url);

            return GetRequest(url, (UnityWebRequest req) =>
            {
                if (req.error == null)
                {
                    Debug.Log("NetworkManager::GetHistocache response: " + req.downloadHandler.text);

                    // response code 200 = either we do not have cached data or cached data has different version from server's, this is the latest data
                    // response code 204 = cached data has the same version as the server's
                    callback(true, req.downloadHandler.text);
                }
                else
                {                    
                    Debug.LogError("NetworkManager::GetHistocache error: " + req.error);
                    callback(false, null);
                }
            });
        }

        public static IEnumerator GetHistocacheCollection(Action<bool, string> callback, string updatedAt = null)
        {
            string url = string.IsNullOrWhiteSpace(updatedAt) ? String.Format("{0}/{1}", baseURL, histocachePath) : String.Format("{0}/{1}?updated_at={2}", baseURL, histocachePath, updatedAt);

            Debug.Log("NetworkManager::GetHistocacheCollection URL: " + url);

            return GetRequest(url, (UnityWebRequest req) =>
            {
                if (req.error == null)
                {
                    Debug.Log("NetworkManager::GetHistocacheCollection response: " + req.downloadHandler.text);

                    // response code 200 = either we do not have cached data or cached data has different version from server's, this is the latest data
                    // response code 204 = cached data has the same version as the server's
                    callback(true, req.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("NetworkManager::GetHistocacheCollection error: " + req.error);
                    callback(false, null);
                }
            });
        }

        public static IEnumerator GetCategoryCollection(Action<bool, string> callback, string updatedAt = null)
        {
            string url = string.IsNullOrWhiteSpace(updatedAt) ? String.Format("{0}/{1}", baseURL, categoryPath) : String.Format("{0}/{1}?updated_at={2}", baseURL, categoryPath, updatedAt);

            Debug.Log("NetworkManager::GetCategoryCollection URL: " + url);

            return GetRequest(url, (UnityWebRequest req) =>
            {
                if (req.error == null)
                {
                    Debug.Log("NetworkManager::GetCategoryCollection response: " + req.downloadHandler.text);

                    // response code 200 = either we do not have cached data or cached data has different version from server's, this is the latest data
                    // response code 204 = cached data has the same version as the server's
                    callback(true, req.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("NetworkManager::GetCategoryCollection error: " + req.error);
                    callback(false, null);
                }
            });
        }
    }
}
