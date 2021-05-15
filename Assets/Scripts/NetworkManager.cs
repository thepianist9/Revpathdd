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
        // public string id;
        // public string categoryId;
        // public string titleDE;
        // public string titleEN;
        // public string descriptionDE;
        // public string descriptionEN;
        // public string captionDE;
        // public string captionEN;
        // public float imageHeight;
        // public float latitude;
        // public float longitude;
        // public string filename;

        public int userId;
        public int id;
        public string title;
        public string body;
    }

    [Serializable]
    public class POIArray
    {
        public POI[] poiArray;
    }

    public class NetworkManager
    {
        private const string url = "https://jsonplaceholder.typicode.com";

        private const string postsPath = "/posts";

        private IEnumerator GetRequest(string url, Action<UnityWebRequest> callback)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Send the request and wait for a response
                yield return request.SendWebRequest();
                callback(request);
            }
        }

        public IEnumerator GetPOIs(Action<UnityWebRequest> callback)
        {
            return GetRequest(url + postsPath, callback);
        }
    }
}
